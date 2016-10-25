using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenICreateAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private EmployerAccountData _accountData;
        private OrchestratorResponse<EmployerAgreementViewModel> _response;
        private const string HashedId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _employerAccountController = new EmployerAccountController(
               _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userWhiteList.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };

            _accountData = new EmployerAccountData
            {
                CompanyName = "Test Corp",
                CompanyNumber = "1244454",
                RegisteredAddress = "1, Test Street",
                DateOfIncorporation = DateTime.Now.AddYears(-10)
            };

            _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>()))
                       .Returns(_accountData);

            _response = new OrchestratorResponse<EmployerAgreementViewModel>()
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        HashedId = HashedId
                    }
                },
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x => x.CreateAccount(It.IsAny<CreateAccountModel>(),It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);
        }

        [Test]
        public async Task ThenIShouldGoBackToTheEmployerTeamPage()
        {
            //Act
            var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("EmployerTeam", result.RouteValues["Controller"]);
        }

        [Test]
        public async Task ThenIShouldGetBackTheAccountId()
        {
            //Act
            var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HashedId, result.RouteValues["accountId"]);
        }
    }
}
