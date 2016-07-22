using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.DeleteInvitationTests
{
    [TestFixture]
    public class WhenICallDeleteInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IAccountTeamRepository> _accountTeamRepository;
        private DeleteInvitationCommandHandler _handler;
        private Invitation _invitation;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _accountTeamRepository = new Mock<IAccountTeamRepository>();
            _handler = new DeleteInvitationCommandHandler(_invitationRepository.Object, _accountTeamRepository.Object);
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local"
            };
        }

        [Test]
        public async Task SuccessfullyDeleteInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);
            _accountTeamRepository.Setup(x => x.GetMembership(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new Membership
            {
                RoleId = (int)Role.Owner
            });

            await _handler.Handle(new DeleteInvitationCommand
            {
                Id = _invitation.Id,
                AccountId = _invitation.AccountId,
                ExternalUserId = "EXT_USER"
            });

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == _invitation.Id && z.Status == InvitationStatus.Deleted)), Times.Once);
        }
    }
}