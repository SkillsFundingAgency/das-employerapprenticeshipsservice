using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    internal class WhenISelectTheNextStepAfterCreatingAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string HashedAccountId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userViewTestingService.Object,
                logger.Object, _flashMessage.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public void ThenIfISelectToAcceptAnAgreementIShouldBeTakenToTheCorrectPage()
        {
            //Act
            var result =  _employerAccountController.GoToNextStep("agreement", HashedAccountId) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("EmployerAgreement", result.RouteValues["Controller"]);
        }

        [Test]
        public void ThenIfISelectTpAddAnOrganisationIShouldBeTakenToTheCorrectPage()
        {
            //Act
            var result = _employerAccountController.GoToNextStep("addOrganisation", HashedAccountId) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AddOrganisation", result.RouteValues["Action"]);
            Assert.AreEqual("Organisation", result.RouteValues["Controller"]);
        }

        [Test]
        public void ThenIfISelectToGoToTheDashboardIShouldBeTakenToTheCorrectPage()
        {
            //Act
            var result = _employerAccountController.GoToNextStep("dashboard", HashedAccountId) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("EmployerTeam", result.RouteValues["Controller"]);
        }

        [Test]
        public void ThenIfISelectAnInvalidStepIShouldBeRediredtedBackToTheStepSelectionPage()
        {
            //Act
            var result = _employerAccountController.GoToNextStep("not_a_valid_step", HashedAccountId) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AccountCreatedNextSteps", result.ViewName);
        }

        [Test]
        public void ThenIfISelectAnInvalidStepIShouldBeNotifiedOfTheError()
        {
            //Act
            var result = _employerAccountController.GoToNextStep("not_a_valid_step", HashedAccountId) as ViewResult;

            var model = result?.Model as OrchestratorResponse<string>;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(FlashMessageSeverityLevel.Error, model.FlashMessage.Severity);
            Assert.AreEqual("Invalid next step chosen", model.FlashMessage.Headline);
        }
    }
}
