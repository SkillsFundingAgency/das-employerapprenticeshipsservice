using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateTransferConnection;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateTransferConnection
{
    public class WhenICreateATransferConnection
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ReceiverHashedAccountId = "XYZ987";
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;
        private const long TransferConnectionId = 333333;

        private CreateTransferConnectionCommandHandler _handler;
        private CreateTransferConnectionCommand _command;
        private long _id;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly Mock<ITransferConnectionRepository> _transferConnectionRepository = new Mock<ITransferConnectionRepository>();
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        private readonly Domain.Data.Entities.Account.Account _receiverAccount = new Domain.Data.Entities.Account.Account { Id = ReceiverAccountId };
        private readonly Domain.Data.Entities.Account.Account _senderAccount = new Domain.Data.Entities.Account.Account { Id = SenderAccountId };
        private TransferConnection _transferConnection;

        [SetUp]
        public void SetUp()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _employerAccountRepository.Setup(r => r.GetAccountByHashedId(ReceiverHashedAccountId)).ReturnsAsync(_receiverAccount);
            _employerAccountRepository.Setup(r => r.GetAccountByHashedId(SenderHashedAccountId)).ReturnsAsync(_senderAccount);

            _transferConnectionRepository.Setup(r => r.Create(It.IsAny<TransferConnection>()))
                .Callback<TransferConnection>(c => _transferConnection = c)
                .ReturnsAsync(TransferConnectionId);

            _handler = new CreateTransferConnectionCommandHandler(_currentUser, _employerAccountRepository.Object, _membershipRepository.Object, _transferConnectionRepository.Object);

            _command = new CreateTransferConnectionCommand
            {
                SenderHashedAccountId = SenderHashedAccountId,
                ReceiverHashedAccountId = ReceiverHashedAccountId
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            _id = await _handler.Handle(_command);

            _membershipRepository.Verify(r => r.GetCaller(SenderHashedAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            _id = await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountByHashedId(ReceiverHashedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetSendersAccount()
        {
            _id = await _handler.Handle(_command);

            _employerAccountRepository.Verify(r => r.GetAccountByHashedId(SenderHashedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldCreateTransferConnection()
        {
            var now = DateTime.UtcNow;

            _id = await _handler.Handle(_command);

            _transferConnectionRepository.Verify(r => r.Create(It.IsAny<TransferConnection>()), Times.Once);

            Assert.That(_transferConnection, Is.Not.Null);
            Assert.That(_transferConnection.SenderUserId, Is.EqualTo(UserId));
            Assert.That(_transferConnection.ReceiverAccountId, Is.EqualTo(ReceiverAccountId));
            Assert.That(_transferConnection.SenderAccountId, Is.EqualTo(SenderAccountId));
            Assert.That(_transferConnection.SenderUserId, Is.EqualTo(UserId));
            Assert.That(_transferConnection.Status, Is.EqualTo(TransferConnectionStatus.Created));
            Assert.That(_transferConnection.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(_transferConnection.ModifiedDate, Is.Null);
        }

        [Test]
        public async Task ThenShouldReturnTransferConnectoinId()
        {
            _id = await _handler.Handle(_command);

            Assert.That(_id, Is.EqualTo(TransferConnectionId));
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public void ThenShouldThrowCommandExceptionIfUserIsNull()
        {
            _employerAccountRepository.Setup(r => r.GetAccountByHashedId(ReceiverHashedAccountId)).ReturnsAsync(null);

            Assert.ThrowsAsync<CommandException<CreateTransferConnectionCommand>>(async () => await _handler.Handle(_command));
        }
    }
}