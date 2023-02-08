using SFA.DAS.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

class WhenIViewNextSteps : ControllerTestBase
{
    private OrganisationController _controller;
    private readonly Mock<OrganisationOrchestrator> _orchestrator = new();
    private readonly Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage = new();

    [SetUp]
    public void Arrange()
    {
        base.Arrange();
        
        _controller = new OrganisationController(
            _orchestrator.Object,
            _flashMessage.Object)
        {
            ControllerContext = ControllerContext
        };
    }

    [Test]
    public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizard()
    {
        //Arrange
        const string userId = "123";
        const string hashedAccountId = "ABC123";
        const string hashedAgreementId = "DGF6756";

        AddUserToContext(userId);

        _orchestrator.Setup(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
            {
                Data = new OrganisationAddedNextStepsViewModel { ShowWizard = true }
            });

        //Act
        var result = _controller.OrganisationAddedNextSteps("test", hashedAccountId, hashedAgreementId).Result as ViewResult;
        var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

        //Assert
        Assert.IsNotNull(model);
        Assert.IsTrue(model.Data.ShowWizard);
        _orchestrator.Verify(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), userId, hashedAccountId, hashedAgreementId), Times.Once);
    }

    [Test]
    public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizardWhenSearching()
    {
        //Arrange
        const string userId = "123";
        const string hashedAccountId = "ABC123";
        const string hashedAgreementId = "DEF456";
        
        AddUserToContext(userId);

        _orchestrator.Setup(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
            {
                Data = new OrganisationAddedNextStepsViewModel { ShowWizard = true }
            });

        //Act
        var result = _controller.OrganisationAddedNextStepsSearch("test", hashedAccountId, hashedAgreementId).Result as ViewResult;
        var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

        //Assert
        Assert.IsNotNull(model);
        Assert.IsTrue(model.Data.ShowWizard);
        _orchestrator.Verify(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), userId, hashedAccountId, hashedAgreementId), Times.Once);
    }

    [Test]
    public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizardWhenIMakeAnIncorrectStepSelection()
    {
        //Arrange
        const string userId = "123";
        const string hashedAccountId = "ABC123";
        const string hashedAgreementId = "DEF456";

        AddUserToContext(userId);

        _orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        //Act
        var result = _controller.GoToNextStep("Not A Step", hashedAccountId, hashedAgreementId, "test").Result as ViewResult;
        var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

        //Assert
        Assert.IsNotNull(model);
        Assert.IsTrue(model.Data.ShowWizard);
        _orchestrator.Verify(x => x.UserShownWizard(userId, hashedAccountId), Times.Once);
    }
}