using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
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
        private const string HashedAccountId = "ABC123";

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
                EmployerRefName = "Scheme 1",
                CompanyNumber = "1244454",
                RegisteredAddress = "1, Test Street",
                DateOfIncorporation = DateTime.Now.AddYears(-10),
                CompanyStatus = "active",
                EmployerRef = "123/ABC",
                RefreshToken = "123",
                AccessToken = "456",
                EmpRefNotFound = true,
                HideBreadcrumb = true
            };

            _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>()))
                       .Returns(_accountData);

            _response = new OrchestratorResponse<EmployerAgreementViewModel>()
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        HashedAccountId = HashedAccountId
                    }
                },
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x => x.CreateAccount(It.IsAny<CreateAccountModel>(), It.IsAny<HttpContextBase>()))
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
            Assert.AreEqual(HashedAccountId, result.RouteValues["HashedAccountId"]);
        }

        [Test]
        public async Task ThenTheBreadCrumbValueIsRemovedFromTempDataIfItExists()
        {
            //Arrange
            _employerAccountController.TempData = new TempDataDictionary { { "HideBreadcrumb", true } };

            //Act
            await _employerAccountController.CreateAccount();

            //Assert
            Assert.IsFalse(_employerAccountController.TempData.ContainsKey("HideBreadcrumb"));
        }

        [Test]
        public async Task ThenTheParamtersArePassedFromTheCookieWhenCreatingTheAccount()
        {
            //Act
            await _employerAccountController.CreateAccount();

            //Assert
            _orchestrator.Verify(x => x.CreateAccount(It.Is<CreateAccountModel>(
                c =>
                    c.CompanyStatus.Equals(_accountData.CompanyStatus) &&
                    c.CompanyName.Equals(_accountData.CompanyName) &&
                    c.RefreshToken.Equals(_accountData.RefreshToken) &&
                    c.CompanyDateOfIncorporation.Equals(_accountData.DateOfIncorporation) &&
                    c.CompanyRegisteredAddress.Equals(_accountData.RegisteredAddress) &&
                    c.AccessToken.Equals(_accountData.AccessToken) &&
                    c.EmployerRef.Equals(_accountData.EmployerRef) &&
                    c.EmployerRefName.Equals(_accountData.EmployerRefName) &&
                    c.CompanyNumber.Equals(_accountData.CompanyNumber)
                ), It.IsAny<HttpContextBase>()));
        }
    }
}
