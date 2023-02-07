using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

class WhenIViewNextSteps
{
    private OrganisationController _controller;
    private Mock<OrganisationOrchestrator> _orchestrator;
    private Mock<IAuthenticationService> _owinWrapper;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {
        _orchestrator = new Mock<OrganisationOrchestrator>();
        _owinWrapper = new Mock<IAuthenticationService>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _controller = new OrganisationController(
            _orchestrator.Object,
            _flashMessage.Object);
    }

    [Test]
    public void ThenIShouldBeToldIfTheUserCanStillSeeTheUserWizard()
    {
        //Arrange
        const string userId = "123";
        const string hashedAccountId = "ABC123";
        const string hashedAgreementId = "DGF6756";

        _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
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

        _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
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

        _owinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(userId);
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