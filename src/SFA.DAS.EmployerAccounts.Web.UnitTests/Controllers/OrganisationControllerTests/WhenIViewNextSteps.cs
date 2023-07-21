namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

class WhenIViewNextSteps : ControllerTestBase
{
    private OrganisationController _controller;
    private Mock<OrganisationOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        _orchestrator = new Mock<OrganisationOrchestrator>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        
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

        _orchestrator.Setup(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
            {
                Data = new OrganisationAddedNextStepsViewModel { ShowWizard = true }
            });

        //Act
        var result = _controller.OrganisationAddedNextSteps("test", hashedAccountId, hashedAgreementId) as ViewResult;
        var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

        //Assert
        Assert.IsNotNull(model);
        Assert.IsTrue(model.Data.ShowWizard);
        _orchestrator.Verify(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), hashedAgreementId), Times.Once);
    }

    [Test]
    public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizardWhenSearching()
    {
        //Arrange
        const string userId = "123";
        const string hashedAccountId = "ABC123";
        const string hashedAgreementId = "DEF456";
        
        AddUserToContext(userId);

        _orchestrator.Setup(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
            {
                Data = new OrganisationAddedNextStepsViewModel { ShowWizard = true }
            });

        //Act
        var result = _controller.OrganisationAddedNextStepsSearch("test", hashedAccountId, hashedAgreementId) as ViewResult;
        var model = result?.Model as OrchestratorResponse<OrganisationAddedNextStepsViewModel>;

        //Assert
        Assert.IsNotNull(model);
        Assert.IsTrue(model.Data.ShowWizard);
        _orchestrator.Verify(x => x.GetOrganisationAddedNextStepViewModel(It.IsAny<string>(), hashedAgreementId), Times.Once);
    }
}