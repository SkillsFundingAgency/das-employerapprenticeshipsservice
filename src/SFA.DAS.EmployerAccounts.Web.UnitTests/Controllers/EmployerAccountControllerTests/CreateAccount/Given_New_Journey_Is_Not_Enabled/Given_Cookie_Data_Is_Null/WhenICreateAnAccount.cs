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
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.CreateAccount.Given_New_Journey_Is_Not_Enabled.Given_Cookie_Data_Is_Null
{
    class WhenICreateAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            new EmployerAccountData
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
                       .Returns((EmployerAccountData)null);

            var mockAuthorization = new Mock<IAuthorizationService>();

            mockAuthorization
                .Setup(
                    m =>
                        m.IsAuthorized(FeatureType.NewRegistration))
                .Returns(false);

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                mockAuthorization.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task Then_I_Should_Be_Redirected_To_Search_Organisatoin_Page()
        {
            //Act
            var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, result.RouteValues["Action"]);
            Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, result.RouteValues["Controller"]);
        }

        [Test]
        public async Task Then_Orchestrator_Create_Account_Is_Not_Called()
        {
            await _employerAccountController.CreateAccount();

            _orchestrator
                .Verify(
                    m => m.CreateAccount(It.IsAny<CreateAccountViewModel>(), It.IsAny<HttpContextBase>())
                    , Times.Never);
        }
    }
}
