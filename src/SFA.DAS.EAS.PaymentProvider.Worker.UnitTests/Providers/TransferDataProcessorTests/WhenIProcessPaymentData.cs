using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.PaymentProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.UnitTests.Providers.TransferDataProcessorTests
{
    public class WhenIProcessPaymentData
    {
        private const long ExpectedAccountId = 545648975;
        private const string ExpectedPeriodEndId = "R16-17";

        private TransferDataProcessor _transferDataProcessor;
        private Mock<IMessageSubscriberFactory> _messageSubscriberFactory;
        private Mock<IMessageSubscriber<PaymentProcessorQueueMessage>> _messageSubscriber;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");
            _cancellationTokenSource = new CancellationTokenSource();

            _messageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            _messageSubscriber = new Mock<IMessageSubscriber<PaymentProcessorQueueMessage>>();

            _messageSubscriberFactory.Setup(x => x.GetSubscriber<PaymentProcessorQueueMessage>())
                .Returns(_messageSubscriber.Object);

            _messageSubscriber.Setup(x => x.ReceiveAsAsync())
                .ReturnsAsync(new FileSystemMessage<PaymentProcessorQueueMessage>(stubDataFile, stubDataFile,
                    new PaymentProcessorQueueMessage
                    {
                        AccountId = ExpectedAccountId,
                        PeriodEndId = ExpectedPeriodEndId
                    })).Callback(() => { _cancellationTokenSource.Cancel(); });

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();

            _transferDataProcessor = new TransferDataProcessor(
                _messageSubscriberFactory.Object, _mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheQueueIsReadForNewMessages()
        {
            //Act
            await _transferDataProcessor.RunAsync(_cancellationTokenSource);

            //Assert
            _messageSubscriber.Verify(x => x.ReceiveAsAsync(), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var fileSystemMessage = new Mock<Message<PaymentProcessorQueueMessage>>();
            _messageSubscriber.Setup(x => x.ReceiveAsAsync())
                            .ReturnsAsync(fileSystemMessage.Object)
                            .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _transferDataProcessor.RunAsync(_cancellationTokenSource);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<RefreshAccountTransfersCommand>()), Times.Never);
        }


        [Test]
        public async Task ThenTheCommandIsCalledIfTheMessageHasData()
        {
            //Act
            await _transferDataProcessor.RunAsync(_cancellationTokenSource);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<RefreshAccountTransfersCommand>(c => c.AccountId.Equals(ExpectedAccountId)
                                && c.PeriodEnd.Equals(ExpectedPeriodEndId))), Times.Once);
        }

        [Test]
        public async Task ThenTheMessageIsDequeuedOnceProcessed()
        {
            //Arrange
            var fileSystemMessage = new Mock<Message<PaymentProcessorQueueMessage>>();
            _messageSubscriber.Setup(x => x.ReceiveAsAsync())
                .ReturnsAsync(fileSystemMessage.Object)
                .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _transferDataProcessor.RunAsync(_cancellationTokenSource);

            //Assert
            fileSystemMessage.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}
