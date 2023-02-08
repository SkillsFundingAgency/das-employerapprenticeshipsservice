using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.InvitationControllerTests;

public class WhenIAcceptAnInvitation : ControllerTestBase
{
    private readonly Mock<InvitationOrchestrator> _invitationOrchestrator = new();
    private InvitationController _controller;
    private readonly EmployerAccountsConfiguration _configuration = new();
    private readonly Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage = new();

    [SetUp]
    public void Arrange()
    {
        base.Arrange();
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