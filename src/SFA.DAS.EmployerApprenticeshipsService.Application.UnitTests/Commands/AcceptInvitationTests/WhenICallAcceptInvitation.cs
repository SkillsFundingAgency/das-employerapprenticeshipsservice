using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AcceptInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.AcceptInvitationTests
{
    [TestFixture]
    public class WhenICallAcceptInvitation
    {
        private AcceptInvitationCommandHandler _handler;
        private Invitation _invitation;
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IUserAccountRepository> _userAccountRepository;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _auditService = new Mock<IAuditService>();
            _handler = new AcceptInvitationCommandHandler(
                _invitationRepository.Object, 
                _membershipRepository.Object, 
                _userAccountRepository.Object,
                _auditService.Object);
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local",
                Status = InvitationStatus.Pending,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(2),
                RoleId = Role.Owner
            };
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public async Task IfExistsAndStatusIsPending()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);
            _userAccountRepository.Setup(x => x.Get(_invitation.Email)).ReturnsAsync(new User());
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(null);

            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = _invitation.Id
            });

            _invitationRepository.Verify(x => x.Accept(_invitation.Email, _invitation.AccountId, (short)_invitation.RoleId), Times.Once);
        }

        [Test]
        public async Task ThenShouldAuditWheninviteHasBeenAccepted()
        {
            //Arrange
            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);
            _userAccountRepository.Setup(x => x.Get(_invitation.Email)).ReturnsAsync(new User());
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = _invitation.Id
            });

            //Assert
            _auditService.Verify(x => x.SendAuditMessage(It.IsAny<EasAuditMessage>()), Times.Once);
        }

        [Test]
        public void IfUserEmailNotFound()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);
            _userAccountRepository.Setup(x => x.Get(_invitation.Email)).ReturnsAsync(null);

            var command = new AcceptInvitationCommand
            {
                Id = _invitation.Id
            };

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfAlreadyAMember()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);
            _userAccountRepository.Setup(x => x.Get(_invitation.Email)).ReturnsAsync(new User());
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new MembershipView());

            var command = new AcceptInvitationCommand
            {
                Id = _invitation.Id
            };

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfNotExists()
        {
            const long invitationId = 1;

            _invitationRepository.Setup(x => x.Get(invitationId)).ReturnsAsync(null);

            var command = new AcceptInvitationCommand
            {
                Id = _invitation.Id
            };

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfStatusIsNotPending()
        {
            _invitation.Status = InvitationStatus.Accepted;

            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);

            var command = new AcceptInvitationCommand
            {
                Id = _invitation.Id
            };

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfHasExpired()
        {
            _invitation.ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-2);

            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);

            var command = new AcceptInvitationCommand
            {
                Id = _invitation.Id
            };

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfInvalidRequest()
        {
            var command = new AcceptInvitationCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }
    }
}