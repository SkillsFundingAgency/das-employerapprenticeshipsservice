﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

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
            var logger = new Mock<ILogger>();

            _employerAccountController = new EmployerAccountController(
               _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userWhiteList.Object, logger.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };

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
                OrganisationType = OrganisationType.Charities
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
                    c.OrganisationReferenceNumber.Equals(_accountData.OrganisationReferenceNumber)
                ), It.IsAny<HttpContextBase>()));
        }

        [Test]
        public async Task ThenIfTheAccountIsSucessfullyCreatedThenTheFlashMessageIsAddedToTempData()
        {
            //Act
            await _employerAccountController.CreateAccount();

            //Assert
            Assert.IsTrue(_employerAccountController.TempData.ContainsKey("successHeader"));

        }



        [Test]
        public async Task ThenIfTheAccountIsSucessfullyCreatedThenTheOrganisationTypeIsAddedToTempData()
        {
            //Act
            await _employerAccountController.CreateAccount();

            //Assert
            Assert.IsTrue(_employerAccountController.TempData.ContainsKey("employerAccountCreated"));
            Assert.AreEqual("Charities", _employerAccountController.TempData["employerAccountCreated"]);

        }
    }
}
