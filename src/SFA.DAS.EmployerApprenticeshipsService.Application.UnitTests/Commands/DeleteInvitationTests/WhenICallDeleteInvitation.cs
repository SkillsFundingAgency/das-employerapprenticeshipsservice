using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.DeleteInvitation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.DeleteInvitationTests
{
    [TestFixture]
    public class WhenICallDeleteInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private DeleteInvitationCommandHandler _handler;
        private Invitation _invitation;
        private DeleteInvitationCommand _command;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _handler = new DeleteInvitationCommandHandler(_invitationRepository.Object, _membershipRepository.Object);
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local"
            };
            _command = new DeleteInvitationCommand
            {
                Email = _invitation.Email,
                HashedId = "1",
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
                RoleId = (int)Role.Owner,
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
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedId, _command.ExternalUserId)).ReturnsAsync(new MembershipView
            {
                RoleId = (int)Role.Viewer
            });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }
    }
}