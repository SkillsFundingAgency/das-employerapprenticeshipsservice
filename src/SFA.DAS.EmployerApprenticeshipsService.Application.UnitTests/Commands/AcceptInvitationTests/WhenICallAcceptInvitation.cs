using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AcceptInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.AcceptInvitationTests
{
    [TestFixture]
    public class WhenICallAcceptInvitation
    {
        private AcceptInvitationCommandHandler _handler;
        private Invitation _invitation;
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IAccountTeamRepository> _accountTeamRepository;
        private Mock<IUserAccountRepository> _userAccountRepository;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _accountTeamRepository = new Mock<IAccountTeamRepository>();
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _handler = new AcceptInvitationCommandHandler(_invitationRepository.Object, _accountTeamRepository.Object, _userAccountRepository.Object);
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local",
                Status = InvitationStatus.Pending,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(2)
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
            _accountTeamRepository.Setup(x => x.GetMembership(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(null);

            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = _invitation.Id
            });

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == _invitation.Id && z.Status == InvitationStatus.Accepted)), Times.Once);
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
            _accountTeamRepository.Setup(x => x.GetMembership(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new Membership());

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