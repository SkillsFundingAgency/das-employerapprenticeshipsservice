using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.LevyDeclarationProvider.Worker.UnitTests.Providers.LevyDeclarationTests
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

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).
                ReturnsAsync(new FileSystemMessage<EmployerRefreshLevyQueueMessage>(stubDataFile, stubDataFile, 
                new EmployerRefreshLevyQueueMessage { AccountId = ExpectedAccountId }));
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(new GetEmployerAccountQuery { AccountId = ExpectedAccountId})).ReturnsAsync(new GetEmployerAccountResponse());

            _dasAccountService = new Mock<IDasAccountService>();
            _dasAccountService.Setup(x => x.GetAccountSchemes(ExpectedAccountId)).ReturnsAsync(new Schemes());

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
    }
}
