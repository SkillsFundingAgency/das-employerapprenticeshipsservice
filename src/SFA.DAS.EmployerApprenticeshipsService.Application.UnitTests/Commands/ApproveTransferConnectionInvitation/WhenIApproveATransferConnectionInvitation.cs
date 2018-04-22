using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.ApproveTransferConnectionInvitation
{
    [TestFixture]
    public class WhenIApproveATransferConnectionInvitation
    {
        private ApproveTransferConnectionInvitationCommandHandler _handler;
        private ApproveTransferConnectionInvitationCommand _command;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionInvitationRepository;
        private Mock<IUserRepository> _userRepository;
        private TransferConnectionInvitation _transferConnectionInvitation;
        private IEntity _entity;
        private User _receiverUser;
        private Domain.Models.Account.Account _senderAccount;
        private Domain.Models.Account.Account _receiverAccount;

        [SetUp]
        public void Arrange()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _transferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();
            _userRepository = new Mock<IUserRepository>();

            _receiverUser = new User
            {
                Id = 123456,
                ExternalId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            };

            _senderAccount = new Domain.Models.Account.Account
            {
                Id = 333333,
                HashedId = "ABC123",
                Name = "Sender"
            };

            _receiverAccount = new Domain.Models.Account.Account
            {
                Id = 222222,
                HashedId = "XYZ987",
                Name = "Receiver"
            };

            _entity = _transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .Build();

            _userRepository.Setup(r => r.GetUserById(_receiverUser.Id)).ReturnsAsync(_receiverUser);
            _employerAccountRepository.Setup(r => r.GetAccountById(_senderAccount.Id)).ReturnsAsync(_senderAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(_receiverAccount.Id)).ReturnsAsync(_receiverAccount);
            _transferConnectionInvitationRepository.Setup(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id)).ReturnsAsync(_transferConnectionInvitation);

            _handler = new ApproveTransferConnectionInvitationCommandHandler(
                _employerAccountRepository.Object,
                _transferConnectionInvitationRepository.Object,
                _userRepository.Object
            );

            _command = new ApproveTransferConnectionInvitationCommand
            {
                AccountId = _receiverAccount.Id,
                UserId = _receiverUser.Id,
                TransferConnectionInvitationId = _transferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountById(_receiverAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_command);

            _userRepository.Verify(r => r.GetUserById(_receiverUser.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetTransferConnectionInvitation()
        {
            await _handler.Handle(_command);

            _transferConnectionInvitationRepository.Verify(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldApproveTransferConnectionInvitation()
        {
            var now = DateTime.UtcNow;

            await _handler.Handle(_command);
            
            Assert.That(_transferConnectionInvitation.Status, Is.EqualTo(TransferConnectionInvitationStatus.Approved));
            Assert.That(_transferConnectionInvitation.Changes.Count, Is.EqualTo(1));

            var change = _transferConnectionInvitation.Changes.Single();

            Assert.That(change.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(change.DeletedByReceiver, Is.Null);
            Assert.That(change.DeletedBySender, Is.Null);
            Assert.That(change.ReceiverAccount, Is.Null);
            Assert.That(change.SenderAccount, Is.Null);
            Assert.That(change.Status, Is.EqualTo(_transferConnectionInvitation.Status));
            Assert.That(change.User, Is.Not.Null);
            Assert.That(change.User.Id, Is.EqualTo(_receiverUser.Id));
        }

        [Test]
        public async Task ThenShouldPublishApprovedTransferConnectionInvitationEvent()
        {
            await _handler.Handle(_command);

            var messages = _entity.GetEvents().ToList();
            var message = messages.OfType<ApprovedTransferConnectionInvitationEvent>().FirstOrDefault();

            Assert.That(messages.Count, Is.EqualTo(1));
            Assert.That(message, Is.Not.Null);
            Assert.That(message.ApprovedByUserExternalId, Is.EqualTo(_receiverUser.ExternalId));
            Assert.That(message.ApprovedByUserId, Is.EqualTo(_receiverUser.Id));
            Assert.That(message.ApprovedByUserName, Is.EqualTo(_receiverUser.FullName));
            Assert.That(message.CreatedAt, Is.EqualTo(_transferConnectionInvitation.Changes.Select(c => c.CreatedDate).Cast<DateTime?>().SingleOrDefault()));
            Assert.That(message.ReceiverAccountHashedId, Is.EqualTo(_receiverAccount.HashedId));
            Assert.That(message.ReceiverAccountId, Is.EqualTo(_receiverAccount.Id));
            Assert.That(message.ReceiverAccountName, Is.EqualTo(_receiverAccount.Name));
            Assert.That(message.SenderAccountHashedId, Is.EqualTo(_senderAccount.HashedId));
            Assert.That(message.SenderAccountId, Is.EqualTo(_senderAccount.Id));
            Assert.That(message.SenderAccountName, Is.EqualTo(_senderAccount.Name));
            Assert.That(message.TransferConnectionInvitationId, Is.EqualTo(_transferConnectionInvitation.Id));
        }

        [Test]
        public void ThenShouldThrowExceptionIfApproverIsNotTheReceiver()
        {
            _command.AccountId = _senderAccount.Id;

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command), "Requires approver account is the receiver account.");
        }

        [Test]
        public void ThenShouldThrowExceptionIfTransferConnectionInvitationIsNotPending()
        {
            _transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _transferConnectionInvitationRepository.Setup(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id)).ReturnsAsync(_transferConnectionInvitation);

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command), "Requires transfer connection invitation is pending.");
        }
    }
}