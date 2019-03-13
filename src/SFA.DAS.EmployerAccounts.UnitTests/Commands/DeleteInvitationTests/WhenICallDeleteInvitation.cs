using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.DeleteInvitationTests
{
    [TestFixture]
    public class WhenICallDeleteInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IMediator> _mediator;
        private DeleteInvitationCommandHandler _handler;
        private Invitation _invitation;
        private DeleteInvitationCommand _command;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _mediator = new Mock<IMediator>();
            _handler = new DeleteInvitationCommandHandler(_invitationRepository.Object, _membershipRepository.Object, _mediator.Object);
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local"
            };
            _command = new DeleteInvitationCommand
            {
                Email = _invitation.Email,
                HashedAccountId = "1",
                ExternalUserId = "EXT_USER"
            };
        }

        [Test]
        public async Task SuccessfullyDeleteInvitation()
        {
            _invitation.Status = InvitationStatus.Pending;
            _invitationRepository.Setup(x => x.Get(_invitation.AccountId, _invitation.Email)).ReturnsAsync(_invitation);
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView
            {
                Role = Role.Owner,
                AccountId = _invitation.AccountId
            });

            await _handler.Handle(_command);

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == _invitation.Id && z.Status == InvitationStatus.Deleted)), Times.Once);
        }

        [Test]
        public void ThrowExceptionWhenInvitationNotFound()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.AccountId, _command.Email)).ReturnsAsync(_invitation);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThrowExceptionWhenCallerIsNotAccountOwner()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.AccountId, _command.Email)).ReturnsAsync(_invitation);
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView
            {
                Role = Role.Viewer
            });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheDeleteInvitationCommandIsValid()
        {
            _invitation.Status = InvitationStatus.Pending;
            _invitationRepository.Setup(x => x.Get(_invitation.AccountId, _invitation.Email)).ReturnsAsync(_invitation);
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView
            {
                Role = Role.Owner,
                AccountId = _invitation.AccountId
            });

            await _handler.Handle(_command);

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(
                    y => y.PropertyName.Equals("Status") && y.NewValue.Equals(InvitationStatus.Deleted.ToString())) != null
                )));

        }
    }
}