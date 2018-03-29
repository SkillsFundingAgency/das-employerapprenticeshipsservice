using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.DeleteTransferConnectionInvitation
{
    [TestFixture]
    public class WhenIDeleteATransferConnectionInvitation
    {
        private DeleteTransferConnectionInvitationCommandHandler _handler;
        private DeleteTransferConnectionInvitationCommand _command;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionInvitationRepository;
        private Mock<IUserRepository> _userRepository;
        private TransferConnectionInvitation _transferConnectionInvitation;
        private IEntity _entity;
        private User _deleterUser;
        private Domain.Data.Entities.Account.Account _senderAccount;
        private Domain.Data.Entities.Account.Account _receiverAccount;

        [SetUp]
        public void Arrange()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _transferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();
            _userRepository = new Mock<IUserRepository>();

            _deleterUser = new User
            {
                Id = 123456,
                ExternalId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            };

            _senderAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 333333,
                HashedId = "ABC123",
                Name = "Sender"
            };

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                HashedId = "XYZ987",
                Name = "Receiver"
            };

            _entity = _transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build();

            _userRepository.Setup(r => r.GetUserById(_deleterUser.Id)).ReturnsAsync(_deleterUser);
            _employerAccountRepository.Setup(r => r.GetAccountById(_receiverAccount.Id)).ReturnsAsync(_receiverAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(_senderAccount.Id)).ReturnsAsync(_senderAccount);
            _transferConnectionInvitationRepository.Setup(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id)).ReturnsAsync(_transferConnectionInvitation);

            _handler = new DeleteTransferConnectionInvitationCommandHandler(
                _employerAccountRepository.Object,
                _transferConnectionInvitationRepository.Object,
                _userRepository.Object
            );

            _command = new DeleteTransferConnectionInvitationCommand
            {
                AccountId = _senderAccount.Id,
                UserId = _deleterUser.Id,
                TransferConnectionInvitationId = _transferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldGetSendersAccount()
        {
            await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountById(_senderAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_command);

            _userRepository.Verify(r => r.GetUserById(_deleterUser.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetTransferConnectionInvitation()
        {
            await _handler.Handle(_command);

            _transferConnectionInvitationRepository.Verify(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldDeleteTransferConnectionInvitation()
        {
            var now = DateTime.UtcNow;

            await _handler.Handle(_command);
            
            Assert.That(_transferConnectionInvitation.Status, Is.EqualTo(TransferConnectionInvitationStatus.Rejected));
            Assert.That(_transferConnectionInvitation.DeletedBySender, Is.True);
            Assert.That(_transferConnectionInvitation.Changes.Count, Is.EqualTo(1));

            var change = _transferConnectionInvitation.Changes.Single();

            Assert.That(change.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(change.DeletedByReceiver, Is.Null);
            Assert.That(change.DeletedBySender, Is.EqualTo(_transferConnectionInvitation.DeletedBySender));
            Assert.That(change.ReceiverAccount, Is.Null);
            Assert.That(change.SenderAccount, Is.Null);
            Assert.That(change.Status, Is.Null);
            Assert.That(change.User, Is.Not.Null);
            Assert.That(change.User.Id, Is.EqualTo(_deleterUser.Id));
        }

        [Test]
        public async Task ThenShouldPublishDeletedTransferConnectionInvitationEvent()
        {
            await _handler.Handle(_command);

            var messages = _entity.GetEvents().ToList();
            var message = messages.OfType<DeletedTransferConnectionInvitationEvent>().FirstOrDefault();

            Assert.That(messages.Count, Is.EqualTo(1));
            Assert.That(message, Is.Not.Null);
            Assert.That(message.DeletedByAccountId, Is.EqualTo(_senderAccount.Id));
            Assert.That(message.DeletedByUserExternalId, Is.EqualTo(_deleterUser.ExternalId));
            Assert.That(message.DeletedByUserId, Is.EqualTo(_deleterUser.Id));
            Assert.That(message.DeletedByUserName, Is.EqualTo(_deleterUser.FullName));
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
        public async Task ThenShouldSetSenderDeletedWhenAccountIsSender()
        {
            _command.AccountId = _senderAccount.Id;
            _transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build();
            _transferConnectionInvitationRepository.Setup(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id)).ReturnsAsync(_transferConnectionInvitation);

            await _handler.Handle(_command);
            Assert.IsTrue(_transferConnectionInvitation.DeletedBySender);
            Assert.IsFalse(_transferConnectionInvitation.DeletedByReceiver);
            Assert.IsTrue(_transferConnectionInvitation.Changes.SingleOrDefault(tcic => tcic.DeletedBySender.HasValue && tcic.DeletedBySender.Value && !tcic.DeletedByReceiver.HasValue) != null);
        }

        [Test]
        public async Task ThenShouldSetSenderDeletedWhenAccountIsReceiver()
        {
            _command.AccountId = _receiverAccount.Id;
            _transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build();
            _transferConnectionInvitationRepository.Setup(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id)).ReturnsAsync(_transferConnectionInvitation);

            await _handler.Handle(_command);
            Assert.IsTrue(_transferConnectionInvitation.DeletedByReceiver);
            Assert.IsFalse(_transferConnectionInvitation.DeletedBySender);
            Assert.IsTrue(_transferConnectionInvitation.Changes.SingleOrDefault(tcic => tcic.DeletedByReceiver.HasValue && tcic.DeletedByReceiver.Value && !tcic.DeletedBySender.HasValue) != null);
        }

        [Test]
        public void ThenShouldThrowExceptionIfTransferConnectionInvitationIsNotPending()
        {
            _transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .Build();

            _transferConnectionInvitationRepository.Setup(r => r.GetTransferConnectionInvitationById(_transferConnectionInvitation.Id)).ReturnsAsync(_transferConnectionInvitation);

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command), "Requires transfer connection invitation is rejected.");
        }
    }
}