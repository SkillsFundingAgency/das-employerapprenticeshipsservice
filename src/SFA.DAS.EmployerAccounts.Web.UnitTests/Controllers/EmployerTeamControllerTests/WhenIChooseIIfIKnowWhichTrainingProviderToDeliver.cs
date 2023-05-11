namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenIChooseIIfIKnowWhichTrainingProviderToDeliver
{
    private EmployerTeamController _controller;

    private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService;
    private Mock<EmployerTeamOrchestratorWithCallToAction> _mockEmployerTeamOrchestrator;

    [SetUp]
    public void Arrange()
    {
        _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestratorWithCallToAction>();

        _controller = new EmployerTeamController(
            _mockCookieStorageService.Object,
            _mockEmployerTeamOrchestrator.Object,
            Mock.Of<IUrlActionHelper>());
    }

    [Test]
    public void IfIChooseYesIContinueTheJourney()
    {
        //Act
        var result = _controller.TriageHaveYouChosenATrainingProvider(new TriageViewModel { TriageOption = TriageOptions.Yes }) as RedirectToActionResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageWillApprenticeshipTrainingStartActionName, result.ActionName);
    }

    [Test]
    public void IfIChooseNoICannotSetupAnApprentice()
    {
        //Act
        var result = _controller.TriageHaveYouChosenATrainingProvider(new TriageViewModel { TriageOption = TriageOptions.No }) as RedirectToActionResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetProviderActionName, result.ActionName);
    }
}