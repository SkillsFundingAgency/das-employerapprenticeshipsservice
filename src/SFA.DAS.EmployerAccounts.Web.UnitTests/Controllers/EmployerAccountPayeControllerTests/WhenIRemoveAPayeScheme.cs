using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests;

public class WhenIRemoveAPayeScheme : ControllerTestBase
{
    private Mock<EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
    private EmployerAccountPayeController _controller;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {

        base.Arrange();
        AddUserToContext();
        
        _employerAccountPayeOrchestrator = new Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator>();
        _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeSchemeViewModel>())).ReturnsAsync(new OrchestratorResponse<RemovePayeSchemeViewModel>());
        
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _controller = new EmployerAccountPayeController(
            Mock.Of<IUrlActionHelper>(),
            _employerAccountPayeOrchestrator.Object,
            _flashMessage.Object,
            Mock.Of<LinkGenerator>())
        {
            ControllerContext = ControllerContext
        };
    }

    [Test]
    public async Task ThenTheOrchestratorIsCalledIfYouConfirmToRemoveTheScheme()
    {
        //Act
        var actual = await _controller.RemovePaye("", new RemovePayeSchemeViewModel { RemoveScheme = 2 });

        //Assert
        _employerAccountPayeOrchestrator.Verify(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeSchemeViewModel>()), Times.Once);
        Assert.IsNotNull(actual);
        var actualRedirect = actual as RedirectToActionResult;
        Assert.IsNotNull(actualRedirect);
        Assert.AreEqual("Index", actualRedirect.ActionName);
        Assert.AreEqual("EmployerAccountPaye", actualRedirect.ControllerName);
        _flashMessage.Verify(x => x.Create(It.Is<FlashMessageViewModel>(c => c.HiddenFlashMessageInformation.Equals("page-paye-scheme-deleted")), It.IsAny<string>(), 1));
    }

    [Test]
    public async Task ThenTheConfirmRemoveSchemeViewIsReturnedIfThereIsAValidationError()
    {
        //Arrange
        _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeSchemeViewModel>())).ReturnsAsync(new OrchestratorResponse<RemovePayeSchemeViewModel> { Status = HttpStatusCode.BadRequest });

        //Act
        var actual = await _controller.RemovePaye("", new RemovePayeSchemeViewModel());

        //Assert
        Assert.IsNotNull(actual);
        var actualView = actual as ViewResult;
        Assert.IsNotNull(actualView);
        Assert.AreEqual("Remove", actualView.ViewName);
    }
}