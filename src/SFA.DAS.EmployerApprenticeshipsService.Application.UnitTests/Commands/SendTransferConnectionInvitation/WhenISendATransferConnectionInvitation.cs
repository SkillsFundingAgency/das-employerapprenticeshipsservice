using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SendTransferConnectionInvitation
{
    [TestFixture]
    public class WhenISendATransferConnectionInvitation
    {
        private SendTransferConnectionInvitationCommandHandler _handler;
        private SendTransferConnectionInvitationCommand _command;
        private long? _id;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IPublicHashingService> _publicHashingService;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionInvitationRepository;
        private Mock<IUserRepository> _userRepository;
        private TransferConnectionInvitation _transferConnectionInvitation;
        private IEntity _entity;
        private Domain.Data.Entities.Account.Account _senderAccount;
        private Domain.Data.Entities.Account.Account _receiverAccount;
        private User _senderUser;

        [SetUp]
        public void Arrange()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _publicHashingService = new Mock<IPublicHashingService>();
            _transferConnectionInvitationRepository = new Mock<ITransferConnectionInvitationRepository>();
            _userRepository = new Mock<IUserRepository>();

            _senderUser = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 123456,
                FirstName = "John",
                LastName = "Doe"
            };

            _senderAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 333333,
                PublicHashedId = "ABC123",
                Name = "Sender"
            };

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                PublicHashedId = "XYZ987",
                Name = "Receiver"
            };

            _publicHashingService.Setup(h => h.DecodeValue(_receiverAccount.PublicHashedId)).Returns(_receiverAccount.Id);
            _userRepository.Setup(r => r.GetUserById(_senderUser.Id)).ReturnsAsync(_senderUser);
            _employerAccountRepository.Setup(r => r.GetAccountById(_senderAccount.Id)).ReturnsAsync(_senderAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(_receiverAccount.Id)).ReturnsAsync(_receiverAccount);
            _transferConnectionInvitationRepository.Setup(r => r.Add(It.IsAny<TransferConnectionInvitation>())).Returns(Task.CompletedTask).Callback<TransferConnectionInvitation>(c => _entity = _transferConnectionInvitation = c);

            _handler = new SendTransferConnectionInvitationCommandHandler(
                _employerAccountRepository.Object,
                _publicHashingService.Object,
                _transferConnectionInvitationRepository.Object,
                _userRepository.Object
            );

            _command = new SendTransferConnectionInvitationCommand
            {
                AccountId = _senderAccount.Id,
                UserId = _senderUser.Id,
                ReceiverAccountPublicHashedId = _receiverAccount.PublicHashedId
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_command);

            _userRepository.Verify(r => r.GetUserById(_senderUser.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetSendersAccount()
        {
            await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountById(_receiverAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountById(_receiverAccount.Id), Times.Once);
        }

        [Test]
        public async Task ThenShouldAddTransferConnectionInvitationToRepository()
        {
            await _handler.Handle(_command);

            _transferConnectionInvitationRepository.Verify(r => r.Add(It.IsAny<TransferConnectionInvitation>()), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnTransferConnectionInvitationId()
        {
            _id = await _handler.Handle(_command);

            Assert.That(_id, Is.Not.Null);
        }

        [Test]
        public void ThenShouldThrowExceptionIfTransferConnectionInvitationAlreadyExists()
        {
            _senderAccount.SentTransferConnectionInvitations.Add(new TransferConnectionInvitationBuilder()
                .WithReceiverAccount(_receiverAccount)
                .Build());

            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command), "Requires transfer connection invitation does not already exist.");
        }

        [Test]
        public async Task ThenShouldNotThrowExceptionIfRejectedTransferConnectionInvitationAlreadyExists()
        {
            _senderAccount.SentTransferConnectionInvitations.Add(new TransferConnectionInvitationBuilder()
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build());

            await _handler.Handle(_command);
        }

        [Test]
        public async Task ThenShouldPublishSentTransferConnectionInvitationEvent()
        {
            _id = await _handler.Handle(_command);

            var messages = _entity.GetEvents().ToList();
            var message = messages.OfType<SentTransferConnectionInvitationEvent>().FirstOrDefault();

            Assert.That(messages.Count, Is.EqualTo(1));
            Assert.That(message, Is.Not.Null);
            Assert.That(message.CreatedAt, Is.EqualTo(_transferConnectionInvitation.Changes.Select(c => c.CreatedDate).Cast<DateTime?>().SingleOrDefault()));
            Assert.That(message.ReceiverAccountHashedId, Is.EqualTo(_receiverAccount.HashedId));
            Assert.That(message.ReceiverAccountId, Is.EqualTo(_receiverAccount.Id));
            Assert.That(message.ReceiverAccountName, Is.EqualTo(_receiverAccount.Name));
            Assert.That(message.SenderAccountHashedId, Is.EqualTo(_senderAccount.HashedId));
            Assert.That(message.SenderAccountId, Is.EqualTo(_senderAccount.Id));
            Assert.That(message.SenderAccountName, Is.EqualTo(_senderAccount.Name));
            Assert.That(message.SentByUserExternalId, Is.EqualTo(_senderUser.ExternalId));
            Assert.That(message.SentByUserId, Is.EqualTo(_senderUser.Id));
            Assert.That(message.SentByUserName, Is.EqualTo(_senderUser.FullName));
            Assert.That(message.TransferConnectionInvitationId, Is.EqualTo(_transferConnectionInvitation.Id));
        }



    }
}