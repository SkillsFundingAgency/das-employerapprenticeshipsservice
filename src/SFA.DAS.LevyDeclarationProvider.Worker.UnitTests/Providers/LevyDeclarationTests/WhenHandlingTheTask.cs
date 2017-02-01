using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker.UnitTests.Providers.LevyDeclarationTests
{
    public class WhenHandlingTheTask
    {
        private const int ExpectedAccountId = 123456;
        private LevyDeclaration _levyDeclaration;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IDasAccountService> _dasAccountService;

        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "true";

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).
                ReturnsAsync(new FileSystemMessage<EmployerRefreshLevyQueueMessage>(stubDataFile, stubDataFile, 
                new EmployerRefreshLevyQueueMessage { AccountId = ExpectedAccountId }));
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(new GetEmployerAccountQuery { AccountId = ExpectedAccountId})).ReturnsAsync(new GetEmployerAccountResponse());
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
                .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
                {
                    UpdateRequired = false
                });
            _mediator.Setup(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>())).ReturnsAsync(Unit.Value);

            _dasAccountService = new Mock<IDasAccountService>();
            _dasAccountService.Setup(x => x.GetAccountSchemes(ExpectedAccountId)).ReturnsAsync(new PayeSchemes());

            _logger = new Mock<ILogger>();

            _levyDeclaration = new LevyDeclaration(_pollingMessageReceiver.Object, _mediator.Object, _logger.Object,_dasAccountService.Object);
        }

        [Test]
        public async Task ThenTheMessageIsReceivedFromTheQueue()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert
            _pollingMessageReceiver.Verify(x=>x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>(),Times.Once);
        }
        

        [Test]
        public async Task ThenTheRebuildDeclarationCommandIsCalledIfThereIsData()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert

        }

        [Test]
        public async Task ThenIfATooManyRequestsExceptionIsThrownItIsHandled()
        {
            //Act
            await _levyDeclaration.Handle();

        }

        [Test]
        public async Task ThenTheSchemesAreRetrunedFromTheService()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert
            _dasAccountService.Verify(x=>x.GetAccountSchemes(ExpectedAccountId), Times.Once);
        } 

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var mockFileMessage = new Mock<Message<EmployerRefreshLevyQueueMessage>>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).ReturnsAsync(mockFileMessage.Object);

            //Act
            await _levyDeclaration.Handle();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()),Times.Never());
            mockFileMessage.Verify(x=>x.CompleteAsync(),Times.Once);
        }

        [Test]
        public async Task ThenWhenHmrcHaveUpdatedTheirEnglishFractionCalculationsIShouldUpdateTheLevyCalculations()
        {
            //Assign
            var expectedAccessToken = "myaccesstoken";
            var expectedEmpref = "123/fgh456";
            _dasAccountService.Setup(x => x.GetAccountSchemes(ExpectedAccountId)).ReturnsAsync(new PayeSchemes { SchemesList = new List<PayeScheme> { new PayeScheme { AccessToken = expectedAccessToken, AccountId = ExpectedAccountId, Id = 1, Ref = expectedEmpref, RefreshToken = "token" } } });
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(expectedEmpref)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(expectedEmpref));


            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.Handle();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheLevyCalculationShouldNotBeUpdateIfTheyAreTheLatest()
        {
            //Act
            await _levyDeclaration.Handle();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenTheLastCalculationDateForEnglishFractionIsUpdatedIfItHasChanged()
        {
            //Arrange
            var expectedAccessToken = "myaccesstoken";
            var expectedEmpref = "123/fgh456";
            _dasAccountService.Setup(x => x.GetAccountSchemes(ExpectedAccountId)).ReturnsAsync(new PayeSchemes { SchemesList = new List<PayeScheme> { new PayeScheme { AccessToken = expectedAccessToken, AccountId = ExpectedAccountId, Id = 1, Ref = expectedEmpref, RefreshToken = "token" } } });
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(expectedEmpref)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(expectedEmpref));
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.Handle();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateEnglishFractionCalculationDateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsFalseNoSchemesAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "false";

            //Act
            await _levyDeclaration.Handle();

            //Assert
            _dasAccountService.Verify(x=>x.GetAccountSchemes(It.IsAny<long>()), Times.Never);
        }
    }
}
