using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SendTransferConnectionInvitation
{
    public class WhenISendATransferConnectionInvitation
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ReceiverHashedAccountId = "XYZ987";
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;
        private const long TransferConnectionInvitationId = 333333;
        private const string SenderAccountName = "Foo";
        private const string ReceiverAccountName = "Bar";
        private const string FirstName = "John";
        private const string LastName = "Doe";

        private SendTransferConnectionInvitationCommandHandler _handler;
        private SendTransferConnectionInvitationCommand _command;
        private long _id;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private readonly Mock<IHashingService> _hashingService = new Mock<IHashingService>();
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly Mock<ITransferConnectionInvitationRepository> _transferConnectionRepository = new Mock<ITransferConnectionInvitationRepository>();
        private Mock<IMessagePublisher> _messagePublisher;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId, UserRef = ExternalUserId, FirstName = FirstName, LastName = LastName };
        private readonly Domain.Data.Entities.Account.Account _senderAccount = new Domain.Data.Entities.Account.Account { Id = SenderAccountId, Name = SenderAccountName };
        private readonly Domain.Data.Entities.Account.Account _receiverAccount = new Domain.Data.Entities.Account.Account { Id = ReceiverAccountId, Name = ReceiverAccountName };
        private TransferConnectionInvitation _transferConnectionInvitation;
        private TransferConnectionInvitationSentMessage _message;

        [SetUp]
        public void SetUp()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();

            _hashingService.Setup(h => h.DecodeValue(ReceiverHashedAccountId)).Returns(ReceiverAccountId);
            _hashingService.Setup(h => h.DecodeValue(SenderHashedAccountId)).Returns(SenderAccountId);
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _employerAccountRepository.Setup(r => r.GetAccountById(SenderAccountId)).ReturnsAsync(_senderAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(ReceiverAccountId)).ReturnsAsync(_receiverAccount);
            _transferConnectionRepository.Setup(r => r.Add(It.IsAny<TransferConnectionInvitation>())).ReturnsAsync(TransferConnectionInvitationId).Callback<TransferConnectionInvitation>(c => _transferConnectionInvitation = c);
            _messagePublisher.Setup(p => p.PublishAsync(It.IsAny<object>())).Returns(Task.CompletedTask).Callback<object>(m => _message = m as TransferConnectionInvitationSentMessage);

            _handler = new SendTransferConnectionInvitationCommandHandler(
                _currentUser,
                _employerAccountRepository.Object,
                _hashingService.Object,
                _membershipRepository.Object,
                _transferConnectionRepository.Object,
                _messagePublisher.Object
            );

            _command = new SendTransferConnectionInvitationCommand
            {
                SenderAccountHashedId = SenderHashedAccountId,
                ReceiverAccountHashedId = ReceiverHashedAccountId
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            _id = await _handler.Handle(_command);

            _membershipRepository.Verify(r => r.GetCaller(SenderHashedAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetSendersAccount()
        {
            _id = await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountById(ReceiverAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            _id = await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountById(ReceiverAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldCreateTransferConnectionInvitation()
        {
            var now = DateTime.UtcNow;

            _id = await _handler.Handle(_command);

            _transferConnectionRepository.Verify(r => r.Add(It.IsAny<TransferConnectionInvitation>()), Times.Once);

            Assert.That(_transferConnectionInvitation, Is.Not.Null);
            Assert.That(_transferConnectionInvitation.SenderUserId, Is.EqualTo(UserId));
            Assert.That(_transferConnectionInvitation.ReceiverAccountId, Is.EqualTo(ReceiverAccountId));
            Assert.That(_transferConnectionInvitation.SenderAccountId, Is.EqualTo(SenderAccountId));
            Assert.That(_transferConnectionInvitation.SenderUserId, Is.EqualTo(UserId));
            Assert.That(_transferConnectionInvitation.Status, Is.EqualTo(TransferConnectionInvitationStatus.Sent));
            Assert.That(_transferConnectionInvitation.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(_transferConnectionInvitation.ModifiedDate, Is.Null);
        }

        [Test]
        public async Task ThenShouldPublishTransferConnectionInvitationSentMessage()
        {
            _id = await _handler.Handle(_command);

            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<TransferConnectionInvitationSentMessage>()), Times.Once);

            Assert.That(_message, Is.Not.Null);
            Assert.That(_message.TransferConnectionId, Is.EqualTo(TransferConnectionInvitationId));
            Assert.That(_message.SenderAccountId, Is.EqualTo(SenderAccountId));
            Assert.That(_message.SenderAccountName, Is.EqualTo(SenderAccountName));
            Assert.That(_message.ReceiverAccountId, Is.EqualTo(ReceiverAccountId));
            Assert.That(_message.ReceiverAccountName, Is.EqualTo(ReceiverAccountName));
            Assert.That(_message.CreatorName, Is.EqualTo($"{FirstName} {LastName}"));
            Assert.That(_message.CreatorUserRef, Is.EqualTo(ExternalUserId));
        }

        [Test]
        public void ThenShouldReturnTransferConnectionInvitationId()
        {
            Assert.That(_id, Is.EqualTo(TransferConnectionInvitationId));
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }
    }
}