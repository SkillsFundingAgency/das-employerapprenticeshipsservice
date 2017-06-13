using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.PaymentProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.PaymentProvider.Worker.UnitTests.Providers.PaymentDataProcessorTests
{
    public class WhenIProcessPaymentData
    {
        private const long ExpectedAccountId = 545648975;
        private const string ExpectedAccountPaymentUrl = "http://someurlForTestData";
        private const string ExpectedPeriodEndId = "R16-17";

        private PaymentDataProcessor _paymentDataProcessor;
        private Mock<IPollingMessageReceiver> _messageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private  CancellationTokenSource _cancellationTokenSource;
        
        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");
            _cancellationTokenSource = new CancellationTokenSource();

            _messageReceiver = new Mock<IPollingMessageReceiver>();
            _mediator = new Mock<IMediator>();

            _messageReceiver.Setup(x => x.ReceiveAsAsync<PaymentProcessorQueueMessage>())
                .ReturnsAsync(new FileSystemMessage<PaymentProcessorQueueMessage>(stubDataFile, stubDataFile,
                    new PaymentProcessorQueueMessage
                    {
                        AccountId = ExpectedAccountId,
                        AccountPaymentUrl = ExpectedAccountPaymentUrl,
                        PeriodEndId = ExpectedPeriodEndId
                    })).Callback(() => {_cancellationTokenSource.Cancel();});

            _logger = new Mock<ILog>();

            _paymentDataProcessor = new PaymentDataProcessor(_messageReceiver.Object, _mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheQueueIsReadForNewMessages()
        {
            //Act
            await _paymentDataProcessor.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _messageReceiver.Verify(x=>x.ReceiveAsAsync<PaymentProcessorQueueMessage>(), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var fileSystemMessage = new Mock<Message<PaymentProcessorQueueMessage>>();
            _messageReceiver.Setup(x => x.ReceiveAsAsync<PaymentProcessorQueueMessage>())
                            .ReturnsAsync(fileSystemMessage.Object)
                            .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _paymentDataProcessor.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<RefreshPaymentDataCommand>()),Times.Never);
        }


        [Test]
        public async Task ThenTheCommandIsCalledIfTheMessageHasData()
        {
            //Act
            await _paymentDataProcessor.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<RefreshPaymentDataCommand>(c=>c.AccountId.Equals(ExpectedAccountId) 
                                && c.PaymentUrl.Equals(ExpectedAccountPaymentUrl) 
                                && c.PeriodEnd.Equals(ExpectedPeriodEndId))), Times.Once);
        }

        [Test]
        public async Task ThenTheMessageIsDequeuedOnceProcessed()
        {
            //Arrange
            var fileSystemMessage = new Mock<Message<PaymentProcessorQueueMessage>>();
            _messageReceiver.Setup(x => x.ReceiveAsAsync<PaymentProcessorQueueMessage>())
                .ReturnsAsync(fileSystemMessage.Object)
                .Callback(() => {_cancellationTokenSource.Cancel();});

            //Act
            await _paymentDataProcessor.RunAsync(_cancellationTokenSource.Token);

            //Assert
            fileSystemMessage.Verify(x=>x.CompleteAsync(),Times.Once);
        }

        [Test]
        public void ThenTheQueueNameIsSetToRefreshPayments()
        {
            //Act Assert
            Assert.AreEqual("refresh_payments",nameof(_paymentDataProcessor.refresh_payments));
        }
    }
}
