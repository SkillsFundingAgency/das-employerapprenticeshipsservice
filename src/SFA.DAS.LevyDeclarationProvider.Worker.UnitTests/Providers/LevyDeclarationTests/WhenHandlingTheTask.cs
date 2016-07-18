using System.IO;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.LevyDeclarationProvider.Worker.UnitTests.Providers.LevyDeclarationTests
{
    public class WhenHandlingTheTask
    {
        private const int ExpecetedEmpref = 123456;
        private LevyDeclaration _levyDeclaration;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<IMessagePublisher> _messagePublisher;
        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).
                ReturnsAsync(new FileSystemMessage<EmployerRefreshLevyQueueMessage>(stubDataFile, stubDataFile, 
                new EmployerRefreshLevyQueueMessage { AccountId = ExpecetedEmpref }));
            _messagePublisher = new Mock<IMessagePublisher>();

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(new GetEmployerAccountQuery { Id = ExpecetedEmpref})).ReturnsAsync(new GetEmployerAccountResponse());

            _levyDeclaration = new LevyDeclaration(_pollingMessageReceiver.Object,_messagePublisher.Object, _mediator.Object);
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
        public async Task ThenTheDeclarationDataIsReceivedForTheQueueMessageId()
        {
            //Act   
            await _levyDeclaration.Handle();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetEmployerAccountQuery>(c=>c.Id.Equals(ExpecetedEmpref))), Times.Once());

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
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).ReturnsAsync(new FileSystemMessage<EmployerRefreshLevyQueueMessage>(It.IsAny<FileInfo>(), It.IsAny<FileInfo>(), new EmployerRefreshLevyQueueMessage{AccountId=0}));

            //Act
            await _levyDeclaration.Handle();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetLevyDeclarationQuery>()),Times.Never());
        }
    }
}
