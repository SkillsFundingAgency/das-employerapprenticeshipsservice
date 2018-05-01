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
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.UnitTests.EventHandlers.CohortApprovedByTransferSenderEventHandlerTests
{
    [TestFixture]
    public class WhenACohortApprovedByTransferSenderEventIsHandled
    {
        private CohortApprovedByTransferSenderEventHandler _handler;
        private CohortApprovedByTransferSender _event;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IMessageSubscriberFactory> _messageSubscriberFactory;
        private Mock<IMessageSubscriber<CohortApprovedByTransferSender>> _messageSubscriber;
        private Mock<IMessage<CohortApprovedByTransferSender>> _logicalMessage;
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
            _messageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            _messageSubscriber = new Mock<IMessageSubscriber<CohortApprovedByTransferSender>>();
            _logicalMessage = new Mock<IMessage<CohortApprovedByTransferSender>>();
            _cancellationTokenSource = new CancellationTokenSource();
            _transferRequestRepository = new Mock<ITransferRequestRepository>();
            _unitOfWorkManager = new Mock<IUnitOfWorkManager>();

            _senderAccount = new Domain.Models.Account.Account
            {
                Id = 111111,
                HashedId = "ABC123"
            };

            _receiverAccount = new Domain.Models.Account.Account
            {
                Id = 222222,
                HashedId = "XYZ987"
            };

            _transferRequest = new TransferRequestBuilder()
                .WithCommitmentId(333333)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferRequestStatus.Pending)
                .Build();

            _event = new CohortApprovedByTransferSender
            {
                CommitmentId = _transferRequest.CommitmentId,
                ReceivingEmployerAccountId = _receiverAccount.Id,
                SendingEmployerAccountId = _senderAccount.Id,
            };

            _logicalMessage.Setup(m => m.Content).Returns(_event);
            _messageSubscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(_logicalMessage.Object).Callback(_cancellationTokenSource.Cancel);
            _messageSubscriberFactory.Setup(s => s.GetSubscriber<CohortApprovedByTransferSender>()).Returns(_messageSubscriber.Object);
            _employerAccountRepository.Setup(r => r.GetAccountById(_senderAccount.Id)).ReturnsAsync(_senderAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(_receiverAccount.Id)).ReturnsAsync(_receiverAccount);
            _transferRequestRepository.Setup(r => r.GetTransferRequestByCommitmentId(_event.CommitmentId)).ReturnsAsync(_transferRequest);

            _handler = new CohortApprovedByTransferSenderEventHandler(
                _employerAccountRepository.Object,
                _messageSubscriberFactory.Object,
                Mock.Of<ILog>(),
                _transferRequestRepository.Object,
                _unitOfWorkManager.Object);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            await _handler.RunAsync(_cancellationTokenSource);

            _employerAccountRepository.Verify(r => r.GetAccountById(_receiverAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetTransferRequest()
        {
            await _handler.RunAsync(_cancellationTokenSource);

            _transferRequestRepository.Verify(r => r.GetTransferRequestByCommitmentId(_transferRequest.CommitmentId), Times.Once);
        }

        [Test]
        public async Task ThenShouldApproveTransferRequest()
        {
            var now = DateTime.UtcNow;

            await _handler.RunAsync(_cancellationTokenSource);

            Assert.That(_transferRequest.ModifiedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(_transferRequest.Status, Is.EqualTo(TransferRequestStatus.Approved));
        }

        [Test]
        public async Task ThenShouldEndUnitOfWork()
        {
            await _handler.RunAsync(_cancellationTokenSource);

            _unitOfWorkManager.Verify(m => m.End(), Times.Once);
        }
    }
}