using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.CreateInvitationTests
{
    [TestFixture]
    public class WhenICallCreateInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private CreateInvitationCommandHandler _handler;
        private CreateInvitationCommand _command;
        private Mock<IAccountTeamRepository> _accountTeamRepository;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _accountTeamRepository = new Mock<IAccountTeamRepository>();
            _handler = new CreateInvitationCommandHandler(_invitationRepository.Object, _accountTeamRepository.Object);
            _command = new CreateInvitationCommand
            {
                AccountId = 101,
                Email = "test.user@test.local",
                Name = "Test User",
                RoleId = Role.Owner
            };
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public async Task ValidCommandFromAccountOwnerCreatesInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new Membership
            {
                RoleId = (int)Role.Owner
            });

            await _handler.Handle(_command);

            _invitationRepository.Verify(x => x.Create(It.Is<Invitation>(m => m.AccountId == _command.AccountId && m.Email == _command.Email && m.Name == _command.Name && m.Status == InvitationStatus.Pending && m.RoleId == _command.RoleId && m.ExpiryDate == DateTimeProvider.Current.UtcNow.Date.AddDays(8))), Times.Once);
        }

        [Test]
        public async Task ValidCommandFromNonAccountOwnerDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new Membership
            {
                RoleId = 0
            });

            await _handler.Handle(_command);

            _invitationRepository.Verify(x => x.Create(It.IsAny<Invitation>()), Times.Never);
        }

        [Test]
        public async Task ValidCommandFromNonAccountUserDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(null);

            await _handler.Handle(_command);

            _invitationRepository.Verify(x => x.Create(It.IsAny<Invitation>()), Times.Never);
        }

        [Test]
        public void InvalidCommandThrowsException()
        {
            var command = new CreateInvitationCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task ValidCommandButExistingDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(new Invitation
            {
                Id = 1,
                AccountId = _command.AccountId,
                Email = _command.Email
            });

            await _handler.Handle(_command);

            _invitationRepository.Verify(x => x.Create(It.IsAny<Invitation>()), Times.Never);
        }
    }
}