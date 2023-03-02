using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.InvitationControllerTests;

public class WhenIAcceptAnInvitation : ControllerTestBase
{
    private Mock<InvitationOrchestrator> _invitationOrchestrator;
    private EmployerAccountsConfiguration _configuration;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private InvitationController _controller;

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        _invitationOrchestrator = new Mock<InvitationOrchestrator>();
        _configuration = new EmployerAccountsConfiguration();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
    }

    [Test]
    public async Task ThenTheInvitationIsAccepted()
    {
        //Arrange
        AddUserToContext("TEST");

        var invitationId = 12345L;
        var invitation = new UserInvitationsViewModel
        {
            Invitations = new List<InvitationView>
            {
                new InvitationView
                {
                    Id = invitationId,
                    AccountName = "Test Account"
                }
            }
        };

        _invitationOrchestrator.Setup(x => x.AcceptInvitation(It.IsAny<long>(), It.IsAny<string>()))
            .Returns(Task.FromResult<object>(null));

        _controller = new InvitationController(
            _invitationOrchestrator.Object,
            _configuration, _flashMessage.Object)
        {
            ControllerContext = ControllerContext
        };

        //Act
        await _controller.Accept(invitationId, invitation);

        //Assert
        _invitationOrchestrator.Verify(x=> x.AcceptInvitation(It.Is<long>(l => l==12345L), It.IsAny<string>()), Times.Once);
    }
}