using System.Configuration;
using System.IO;
using System.Threading;
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
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker.UnitTests.Providers.LevyDeclarationTests
{
    public class WhenHandlingTheTask
    {
        private const int ExpectedAccountId = 123456;
        private const string ExpectedPayeRef = "YHG/123LJH";
        private LevyDeclaration _levyDeclaration;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IDasAccountService> _dasAccountService;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            _cancellationTokenSource = new CancellationTokenSource();

            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "both";

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).
                ReturnsAsync(new FileSystemMessage<EmployerRefreshLevyQueueMessage>(stubDataFile, stubDataFile,
                new EmployerRefreshLevyQueueMessage { AccountId = ExpectedAccountId, PayeRef = ExpectedPayeRef})).Callback(() => { _cancellationTokenSource.Cancel(); });

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(new GetEmployerAccountQuery { AccountId = ExpectedAccountId })).ReturnsAsync(new GetEmployerAccountResponse());
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
                .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
                {
                    UpdateRequired = false
                });
            _mediator.Setup(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>())).ReturnsAsync(Unit.Value);

            _dasAccountService = new Mock<IDasAccountService>();

            _logger = new Mock<ILogger>();

            _levyDeclaration = new LevyDeclaration(_pollingMessageReceiver.Object, _mediator.Object, _logger.Object, _dasAccountService.Object);
        }

        [Test]
        public async Task ThenTheMessageIsReceivedFromTheQueue()
        {
            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _pollingMessageReceiver.Verify(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>(), Times.Once);
        }

        
        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var mockFileMessage = new Mock<Message<EmployerRefreshLevyQueueMessage>>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>())
                                   .ReturnsAsync(mockFileMessage.Object)
                                   .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()), Times.Never());
            mockFileMessage.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task ThenIfTheConfigIsSetNotToProcessAndTheMessageIsEmptyTheCompleteAsyncIsNotCalled()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "none";
            var mockFileMessage = new Mock<Message<EmployerRefreshLevyQueueMessage>> {DefaultValue = DefaultValue.Empty};

            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>())
                                   .ReturnsAsync(mockFileMessage.Object)
                                   .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()), Times.Never());
            mockFileMessage.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Test]
        public async Task ThenWhenHmrcHaveUpdatedTheirEnglishFractionCalculationsIShouldUpdateTheLevyCalculations()
        {
            //Assign
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));


            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Once);
        }
        
        [Test]
        public async Task ThenTheLastCalculationDateForEnglishFractionIsUpdatedIfItHasChanged()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateEnglishFractionCalculationDateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsNoneNoSchemesAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "none";

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _dasAccountService.Verify(x => x.GetAccountSchemes(It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsFractionsThenOnlyFractionsAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "fractions";
            var expectedAccessToken = "myaccesstoken";
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));


            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<RefreshEmployerLevyDataCommand>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()), Times.Never);

        }


        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsDeclarationsThenOnlyDeclarationsAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "declarations";
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Never);
            _dasAccountService.Verify(x => x.UpdatePayeScheme(ExpectedPayeRef), Times.Never);
            _mediator.Verify(x => x.SendAsync(It.IsAny<RefreshEmployerLevyDataCommand>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()), Times.Once);

        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenTheCallToUpdateExistingPayeInformationIsMadeForNewAndExistingSchemes(bool updateRequired)
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = updateRequired
               });
            
            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _dasAccountService.Verify(x => x.UpdatePayeScheme(ExpectedPayeRef), Times.Once);
        }
    }
}
