using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.InvitationControllerTests;

public class WhenViewingInvitations : ControllerTestBase
{
    private readonly Mock<InvitationOrchestrator> _invitationOrchestrator = new();
    private InvitationController _controller;
    private readonly EmployerAccountsConfiguration _configuration = new() ;
    private readonly Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage= new();

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

       _controller = new InvitationController(
            _invitationOrchestrator.Object,
            _configuration,
            _flashMessage.Object)
       {
           ControllerContext = ControllerContext
       };
    }

    [Test]
    public void ThenTheUserIsShownTheIndexWhenNotAuthenticated()
    {
        //Arrange
        AddEmptyUserToContext();
        
        //Act
        var actual = _controller.Invite();

        //Assert
        Assert.IsNotNull(actual);
    }

    [Test]
    public void ThenTheUserIsRedirectedToTheServiceLandingPageWhenAuthenticated()
    {
        //Arrange
        AddUserToContext("my_user_id");
        
        //Act
        var actual = _controller.Invite();

        //Assert
        Assert.IsNotNull(actual);
        var actualRedirectResult = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirectResult);
        Assert.AreEqual("Index",actualRedirectResult.ActionName);
        Assert.AreEqual("Home",actualRedirectResult.ControllerName);
    }

    [Test]
    public async Task ThenTheCorrectInvitationIsRetrieved()
    {
        //Arrange
        AddUserToContext("TEST");
        _invitationOrchestrator.Setup(x => x.GetInvitation(It.Is<string>(i => i == "123")))
            .ReturnsAsync(new OrchestratorResponse<InvitationView> { Data = new InvitationView() });


        //Act
        var actual = await _controller.Details("123");

        //Assert
        _invitationOrchestrator.Verify(x => x.GetInvitation(It.Is<string>(i => i == "123")));
        Assert.IsNotNull(actual);
        var viewResult = actual as ViewResult;
        Assert.IsNotNull(viewResult);
    }
}