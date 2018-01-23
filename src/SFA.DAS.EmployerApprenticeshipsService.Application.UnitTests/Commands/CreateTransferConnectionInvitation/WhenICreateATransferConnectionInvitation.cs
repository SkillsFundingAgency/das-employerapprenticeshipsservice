using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateTransferConnectionInvitation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateTransferConnectionInvitation
{
    public class WhenICreateATransferConnectionInvitation
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ReceiverHashedAccountId = "XYZ987";
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;
        private const long TransferConnectionInvitationId = 333333;

        private CreateTransferConnectionInvitationCommandHandler _handler;
        private CreateTransferConnectionInvitationCommand _command;
        private long _id;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly Mock<ITransferConnectionInvitationRepository> _transferConnectionRepository = new Mock<ITransferConnectionInvitationRepository>();
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        private readonly Domain.Data.Entities.Account.Account _receiverAccount = new Domain.Data.Entities.Account.Account { Id = ReceiverAccountId };
        private readonly Domain.Data.Entities.Account.Account _senderAccount = new Domain.Data.Entities.Account.Account { Id = SenderAccountId };
        private TransferConnectionInvitation _transferConnectionInvitation;

        [SetUp]
        public void SetUp()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _employerAccountRepository.Setup(r => r.GetAccountByHashedId(ReceiverHashedAccountId)).ReturnsAsync(_receiverAccount);
            _employerAccountRepository.Setup(r => r.GetAccountByHashedId(SenderHashedAccountId)).ReturnsAsync(_senderAccount);

            _transferConnectionRepository.Setup(r => r.Create(It.IsAny<TransferConnectionInvitation>()))
                .Callback<TransferConnectionInvitation>(c => _transferConnectionInvitation = c)
                .ReturnsAsync(TransferConnectionInvitationId);

            _handler = new CreateTransferConnectionInvitationCommandHandler(_currentUser, _employerAccountRepository.Object, _membershipRepository.Object, _transferConnectionRepository.Object);

            _command = new CreateTransferConnectionInvitationCommand
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
        public async Task ThenShouldCreateTransferConnectionInvitation()
        {
            var now = DateTime.UtcNow;

            _id = await _handler.Handle(_command);

            _transferConnectionRepository.Verify(r => r.Create(It.IsAny<TransferConnectionInvitation>()), Times.Once);

            Assert.That(_transferConnectionInvitation, Is.Not.Null);
            Assert.That(_transferConnectionInvitation.SenderUserId, Is.EqualTo(UserId));
            Assert.That(_transferConnectionInvitation.ReceiverAccountId, Is.EqualTo(ReceiverAccountId));
            Assert.That(_transferConnectionInvitation.SenderAccountId, Is.EqualTo(SenderAccountId));
            Assert.That(_transferConnectionInvitation.SenderUserId, Is.EqualTo(UserId));
            Assert.That(_transferConnectionInvitation.Status, Is.EqualTo(TransferConnectionInvitationStatus.Created));
            Assert.That(_transferConnectionInvitation.CreatedDate, Is.GreaterThanOrEqualTo(now));
            Assert.That(_transferConnectionInvitation.ModifiedDate, Is.Null);
        }

        [Test]
        public async Task ThenShouldReturnTransferConnectoinInvitationId()
        {
            _id = await _handler.Handle(_command);

            Assert.That(_id, Is.EqualTo(TransferConnectionInvitationId));
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

            Assert.ThrowsAsync<DomainException<CreateTransferConnectionInvitationCommand>>(async () => await _handler.Handle(_command));
        }
    }
}