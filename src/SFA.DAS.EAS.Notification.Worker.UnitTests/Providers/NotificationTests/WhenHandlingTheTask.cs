using System.IO;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.EAS.Notification.Worker.UnitTests.Providers.NotificationTests
{
    
    public class WhenHandlingTheTask
    {
        public int ExpectedMessageId = 1;

        private Worker.Providers.Notification _notification;
        private Mock<ILogger> _logger;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private FileInfo _stubDataFile;

        [SetUp]
        public void Arrange()
        {
            _stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<SendNotificationQueueMessage>()).ReturnsAsync(new FileSystemMessage<SendNotificationQueueMessage>(_stubDataFile, _stubDataFile,
                new SendNotificationQueueMessage { Id = ExpectedMessageId }));
            _logger = new Mock<ILogger>();
            _mediator = new Mock<IMediator>();

            _notification = new Worker.Providers.Notification(_logger.Object, _pollingMessageReceiver.Object, _mediator.Object);
        }

        [Test]
        public async Task ThenTheQueueIsRead()
        {
            //Act
            await _notification.Handle();

            //Assert
            _pollingMessageReceiver.Verify(x=>x.ReceiveAsAsync<SendNotificationQueueMessage>(), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfThereIsNoMessage()
        {
            //Arrange
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<SendNotificationQueueMessage>()).ReturnsAsync(null);

            //Act
            await _notification.Handle();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<ProcessNotificationCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenTheCommandIsCalledWhenThereIsAMessageOnTheQueue()
        {
            //Act
            await _notification.Handle();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<ProcessNotificationCommand>(c=>c.Id.Equals(ExpectedMessageId))), Times.Once);
        }


        [Test]
        public async Task ThenAnInfoLevelLogIsRecordedWhenAMessageIsSentToBeProcessed()
        {
            //Act
            await _notification.Handle();

            //Assert
            _logger.Verify(x => x.Info($"Processing notification id: {ExpectedMessageId}"), Times.Once);
        }

        [Test]
        public async Task ThenTheCompleteMessageIsCalledWhenAMessageHasBeenProcessed()
        {
            //Arrange
            var mockMessage = new Mock<Message<SendNotificationQueueMessage>>(new SendNotificationQueueMessage {Id = ExpectedMessageId});
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<SendNotificationQueueMessage>()).ReturnsAsync(mockMessage.Object);

            //Act
            await _notification.Handle();

            //Assert
            mockMessage.Verify(x=>x.CompleteAsync(),Times.Once);
        }
    }
}
