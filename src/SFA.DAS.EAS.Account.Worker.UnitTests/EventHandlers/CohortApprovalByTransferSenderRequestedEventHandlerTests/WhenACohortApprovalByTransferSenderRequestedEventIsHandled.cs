using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Events;
using SFA.DAS.EAS.Account.Worker.EventHandlers;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferRequests;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.UnitTests.EventHandlers.CohortApprovalByTransferSenderRequestedEventHandlerTests
{
    [TestFixture]
    public class WhenACohortApprovalByTransferSenderRequestedEventIsHandled
    {
        private const string CommitmentHashedId = "ABC123";

        private CohortApprovalByTransferSenderRequestedEventHandler _handler;
        private CohortApprovalByTransferSenderRequested _event;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IHashingService> _hashingService;
        private Mock<IMessageSubscriberFactory> _messageSubscriberFactory;
        private Mock<IMessageSubscriber<CohortApprovalByTransferSenderRequested>> _messageSubscriber;
        private Mock<IMessage<CohortApprovalByTransferSenderRequested>> _logicalMessage;
        private CancellationTokenSource _cancellationTokenSource;
        private Mock<ITransferRequestRepository> _transferRequestRepository;
        private TransferRequest _transferRequest;
        private Domain.Models.Account.Account _senderAccount;
        private Domain.Models.Account.Account _receiverAccount;
        private Mock<IUnitOfWorkManager> _unitOfWorkManager;

        [SetUp]
        public void Arrange()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _hashingService = new Mock<IHashingService>();
            _messageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            _messageSubscriber = new Mock<IMessageSubscriber<CohortApprovalByTransferSenderRequested>>();
            _logicalMessage = new Mock<IMessage<CohortApprovalByTransferSenderRequested>>();
            _cancellationTokenSource = new CancellationTokenSource();
            _transferRequestRepository = new Mock<ITransferRequestRepository>();
            _unitOfWorkManager = new Mock<IUnitOfWorkManager>();

            _senderAccount = new Domain.Models.Account.Account
            {
                Id = 111111,
                PublicHashedId = "DEF456"
            };

            _receiverAccount = new Domain.Models.Account.Account
            {
                Id = 222222,
                PublicHashedId = "GHI789"
            };

            _event = new CohortApprovalByTransferSenderRequested
            {
                CommitmentId = 333333,
                ReceivingEmployerAccountId = _receiverAccount.Id,
                SendingEmployerAccountId = _senderAccount.Id,
                TransferCost = 123.456m
            };

            _logicalMessage.Setup(m => m.Content).Returns(_event);
            _messageSubscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(_logicalMessage.Object).Callback(_cancellationTokenSource.Cancel);
            _messageSubscriberFactory.Setup(s => s.GetSubscriber<CohortApprovalByTransferSenderRequested>()).Returns(_messageSubscriber.Object);
            _hashingService.Setup(h => h.HashValue(_event.CommitmentId)).Returns(CommitmentHashedId);
            _employerAccountRepository.Setup(r => r.GetAccountById(_senderAccount.Id)).ReturnsAsync(_senderAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(_receiverAccount.Id)).ReturnsAsync(_receiverAccount);
            _transferRequestRepository.Setup(r => r.Add(It.IsAny<TransferRequest>())).Returns(Task.CompletedTask).Callback<TransferRequest>(c => _transferRequest = c);

            _handler = new CohortApprovalByTransferSenderRequestedEventHandler(
                _employerAccountRepository.Object,
                _hashingService.Object,
                _messageSubscriberFactory.Object,
                Mock.Of<ILog>(),
                _transferRequestRepository.Object,
                _unitOfWorkManager.Object);
        }

        [Test]
        public async Task ThenShouldGetSendersAccount()
        {
            await _handler.RunAsync(_cancellationTokenSource);

            _employerAccountRepository.Verify(r => r.GetAccountById(_senderAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            await _handler.RunAsync(_cancellationTokenSource);

            _employerAccountRepository.Verify(r => r.GetAccountById(_receiverAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldCreateTransferRequest()
        {
            var now = DateTime.UtcNow;

            await _handler.RunAsync(_cancellationTokenSource);

            _transferRequestRepository.Verify(r => r.Add(It.IsAny<TransferRequest>()), Times.Once);

            Assert.That(_transferRequest.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(_transferRequest.CommitmentId, Is.EqualTo(_event.CommitmentId));
            Assert.That(_transferRequest.CommitmentHashedId, Is.EqualTo(CommitmentHashedId));
            Assert.That(_transferRequest.ModifiedDate, Is.Null);
            Assert.That(_transferRequest.ReceiverAccount, Is.SameAs(_receiverAccount));
            Assert.That(_transferRequest.SenderAccount, Is.SameAs(_senderAccount));
            Assert.That(_transferRequest.Status, Is.EqualTo(TransferRequestStatus.Pending));
            Assert.That(_transferRequest.TransferCost, Is.EqualTo(_event.TransferCost));
        }

        [Test]
        public async Task ThenShouldEndUnitOfWork()
        {
            await _handler.RunAsync(_cancellationTokenSource);

            _unitOfWorkManager.Verify(m => m.End(), Times.Once);
        }
    }
}