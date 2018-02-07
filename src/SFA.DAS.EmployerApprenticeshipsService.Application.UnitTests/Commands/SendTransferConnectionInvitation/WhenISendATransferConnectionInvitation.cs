using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SendTransferConnectionInvitation
{
    [TestFixture]
    public class WhenISendATransferConnectionInvitation
    {
        private SendTransferConnectionInvitationCommandHandler _handler;
        private SendTransferConnectionInvitationCommand _command;
        private long? _id;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IHashingService> _hashingService;
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
            _hashingService = new Mock<IHashingService>();
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
                Name = "Sender"
            };

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                Name = "Receiver"
            };

            _hashingService.Setup(h => h.DecodeValue(_receiverAccount.HashedId)).Returns(_receiverAccount.Id);
            _userRepository.Setup(r => r.GetUserById(_senderUser.Id)).ReturnsAsync(_senderUser);
            _employerAccountRepository.Setup(r => r.GetAccountById(_senderAccount.Id)).ReturnsAsync(_senderAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(_receiverAccount.Id)).ReturnsAsync(_receiverAccount);
            _transferConnectionInvitationRepository.Setup(r => r.Add(It.IsAny<TransferConnectionInvitation>())).Returns(Task.CompletedTask).Callback<TransferConnectionInvitation>(c => _entity = _transferConnectionInvitation = c);

            _handler = new SendTransferConnectionInvitationCommandHandler(
                _employerAccountRepository.Object,
                _hashingService.Object,
                _transferConnectionInvitationRepository.Object,
                _userRepository.Object
            );

            _command = new SendTransferConnectionInvitationCommand
            {
                AccountId = _senderAccount.Id,
                UserId = _senderUser.Id,
                ReceiverAccountHashedId = _receiverAccount.HashedId
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
        public async Task ThenShouldCreateTransferConnectionInvitation()
        {
            var now = DateTime.UtcNow;

            _id = await _handler.Handle(_command);

            _transferConnectionInvitationRepository.Verify(r => r.Add(It.IsAny<TransferConnectionInvitation>()), Times.Once);

            Assert.That(_transferConnectionInvitation, Is.Not.Null);
            Assert.That(_transferConnectionInvitation.SenderUser.Id, Is.EqualTo(_senderUser.Id));
            Assert.That(_transferConnectionInvitation.ReceiverAccount.Id, Is.EqualTo(_receiverAccount.Id));
            Assert.That(_transferConnectionInvitation.SenderAccount.Id, Is.EqualTo(_senderAccount.Id));
            Assert.That(_transferConnectionInvitation.SenderUser.Id, Is.EqualTo(_senderUser.Id));
            Assert.That(_transferConnectionInvitation.Status, Is.EqualTo(TransferConnectionInvitationStatus.Pending));
            Assert.That(_transferConnectionInvitation.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(_transferConnectionInvitation.ModifiedDate, Is.Null);
        }

        [Test]
        public async Task ThenShouldPublishTransferConnectionInvitationSentMessage()
        {
            _id = await _handler.Handle(_command);
            
            var messages = _entity.GetEvents().ToList();
            var message = messages.OfType<TransferConnectionInvitationSentMessage>().FirstOrDefault();

            Assert.That(messages.Count, Is.EqualTo(1));
            Assert.That(message, Is.Not.Null);
            Assert.That(message.TransferConnectionId, Is.EqualTo(_transferConnectionInvitation.Id));
            Assert.That(message.TransferConnectionInvitationId, Is.EqualTo(_transferConnectionInvitation.Id));
            Assert.That(message.SenderAccountId, Is.EqualTo(_senderAccount.Id));
            Assert.That(message.SenderAccountName, Is.EqualTo(_senderAccount.Name));
            Assert.That(message.ReceiverAccountId, Is.EqualTo(_receiverAccount.Id));
            Assert.That(message.ReceiverAccountName, Is.EqualTo(_receiverAccount.Name));
            Assert.That(message.CreatorUserRef, Is.EqualTo(_senderUser.ExternalId.ToString()));
            Assert.That(message.SenderUserExternalId, Is.EqualTo(_senderUser.ExternalId));
            Assert.That(message.CreatorName, Is.EqualTo(_senderUser.FullName));
            Assert.That(message.SenderUserName, Is.EqualTo(_senderUser.FullName));
        }

        [Test]
        public void ThenShouldReturnTransferConnectionInvitationId()
        {
            Assert.That(_id, Is.Not.Null);
        }
    }
}