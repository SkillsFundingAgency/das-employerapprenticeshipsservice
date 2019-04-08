using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenICreateAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private EmployerAccountData _accountData;
        private OrchestratorResponse<EmployerAgreementViewModel> _response;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string HashedAccountId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();


            _accountData = new EmployerAccountData
            {
                EmployerAccountOrganisationData = new EmployerAccountOrganisationData
                { 
                OrganisationName = "Test Corp",
                OrganisationReferenceNumber = "1244454",
                OrganisationRegisteredAddress = "1, Test Street",
                OrganisationDateOfInception = DateTime.Now.AddYears(-10),
                OrganisationStatus = "active",
                OrganisationType = OrganisationType.Charities,
                Sector = "Public"
                },
                EmployerAccountPayeRefData = new EmployerAccountPayeRefData
                { 
                PayeReference = "123/ABC",
                EmployerRefName = "Scheme 1",
                RefreshToken = "123",
                AccessToken = "456",
                EmpRefNotFound = true,
                }
            };

            _orchestrator.Setup(x => x.GetCookieData())
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

            _orchestrator.Setup(x => x.CreateAccount(It.IsAny<CreateAccountViewModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object, _orchestrator.Object, _userViewTestingService.Object, 
                logger.Object, _flashMessage.Object, Mock.Of<IMediator>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenIShouldGoBackToTheEmployerTeamPage()
        {
            //Act
            var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

            //Assert
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
        public async Task ThenTheParamtersArePassedFromTheCookieWhenCreatingTheAccount()
        {
            //Act
            await _employerAccountController.CreateAccount();

            //Assert
            _orchestrator.Verify(x => x.CreateAccount(It.Is<CreateAccountViewModel>(
                c =>
                    c.OrganisationStatus.Equals(_accountData.EmployerAccountOrganisationData.OrganisationStatus) &&
                    c.OrganisationName.Equals(_accountData.EmployerAccountOrganisationData.OrganisationName) &&
                    c.RefreshToken.Equals(_accountData.EmployerAccountPayeRefData.RefreshToken) &&
                    c.OrganisationDateOfInception.Equals(_accountData.EmployerAccountOrganisationData.OrganisationDateOfInception) &&
                    c.OrganisationAddress.Equals(_accountData.EmployerAccountOrganisationData.OrganisationRegisteredAddress) &&
                    c.AccessToken.Equals(_accountData.EmployerAccountPayeRefData.AccessToken) &&
                    c.PayeReference.Equals(_accountData.EmployerAccountPayeRefData.PayeReference) &&
                    c.EmployerRefName.Equals(_accountData.EmployerAccountPayeRefData.EmployerRefName) &&
                    c.Sector.Equals(_accountData.EmployerAccountOrganisationData.Sector) &&
                    c.OrganisationReferenceNumber.Equals(_accountData.EmployerAccountOrganisationData.OrganisationReferenceNumber)
                ), It.IsAny<HttpContextBase>()));
        }
    }
}
