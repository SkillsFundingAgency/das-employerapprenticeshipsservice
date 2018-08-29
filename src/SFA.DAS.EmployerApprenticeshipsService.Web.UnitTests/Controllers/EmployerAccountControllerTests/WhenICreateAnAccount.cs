﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
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
                OrganisationName = "Test Corp",
                EmployerRefName = "Scheme 1",
                OrganisationReferenceNumber = "1244454",
                OrganisationRegisteredAddress = "1, Test Street",
                OrganisationDateOfInception = DateTime.Now.AddYears(-10),
                OrganisationStatus = "active",
                PayeReference = "123/ABC",
                RefreshToken = "123",
                AccessToken = "456",
                EmpRefNotFound = true,
                OrganisationType = OrganisationType.Charities,
                Sector = "Public"
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

            _orchestrator.Setup(x => x.CreateAccount(It.IsAny<CreateAccountViewModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userViewTestingService.Object, 
                logger.Object, _flashMessage.Object)
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
                    c.OrganisationStatus.Equals(_accountData.OrganisationStatus) &&
                    c.OrganisationName.Equals(_accountData.OrganisationName) &&
                    c.RefreshToken.Equals(_accountData.RefreshToken) &&
                    c.OrganisationDateOfInception.Equals(_accountData.OrganisationDateOfInception) &&
                    c.OrganisationAddress.Equals(_accountData.OrganisationRegisteredAddress) &&
                    c.AccessToken.Equals(_accountData.AccessToken) &&
                    c.PayeReference.Equals(_accountData.PayeReference) &&
                    c.EmployerRefName.Equals(_accountData.EmployerRefName) &&
                    c.Sector.Equals(_accountData.Sector) &&
                    c.OrganisationReferenceNumber.Equals(_accountData.OrganisationReferenceNumber)
                ), It.IsAny<HttpContextBase>()));
        }
    }
}
