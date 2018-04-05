using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateTransferTransactions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.PaymentProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.UnitTests.Providers.TransferTransactionProcessorTests
{
    public class WhenIProcessAccountTransfers
    {
        private const long ExpectedAccountId = 545648975;
        private const string ExpectedPeriodEndId = "R16-17";

        private TransferTransactionProcessor _transferDataProcessor;
        private Mock<IMessageSubscriberFactory> _messageSubscriberFactory;
        private Mock<IMessageSubscriber<AccountTransfersProcessingCompletedMessage>> _messageSubscriber;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private CancellationTokenSource _cancellationTokenSource;


        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");
            _cancellationTokenSource = new CancellationTokenSource();

            _messageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            _messageSubscriber = new Mock<IMessageSubscriber<AccountTransfersProcessingCompletedMessage>>();

            _messageSubscriberFactory.Setup(x => x.GetSubscriber<AccountTransfersProcessingCompletedMessage>())
                .Returns(_messageSubscriber.Object);

            _messageSubscriber.Setup(x => x.ReceiveAsAsync())
                .ReturnsAsync(new FileSystemMessage<AccountTransfersProcessingCompletedMessage>(stubDataFile, stubDataFile,
                    new AccountTransfersProcessingCompletedMessage
                    {
                        AccountId = ExpectedAccountId,
                        PeriodEnd = ExpectedPeriodEndId
                    }
                    )).Callback(() => { _cancellationTokenSource.Cancel(); });

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();

            _transferDataProcessor = new TransferTransactionProcessor(
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
            var fileSystemMessage = new Mock<Message<AccountTransfersProcessingCompletedMessage>>();
            _messageSubscriber.Setup(x => x.ReceiveAsAsync())
                            .ReturnsAsync(fileSystemMessage.Object)
                            .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _transferDataProcessor.RunAsync(_cancellationTokenSource);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateTransferTransactionsCommand>()), Times.Never);
        }


        [Test]
        public async Task ThenTheCommandIsCalledIfTheMessageHasData()
        {
            //Act
            await _transferDataProcessor.RunAsync(_cancellationTokenSource);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateTransferTransactionsCommand>(c => c.ReceiverAccountId.Equals(ExpectedAccountId)
                                && c.PeriodEnd.Equals(ExpectedPeriodEndId))), Times.Once);
        }

        [Test]
        public async Task ThenTheMessageIsDequeuedOnceProcessed()
        {
            //Arrange
            var fileSystemMessage = new Mock<Message<AccountTransfersProcessingCompletedMessage>>();
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
