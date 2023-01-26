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
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.CreateAccount.Given_New_Journey_Is_Enabled.Given_Return_Url_Cookie_Is_Present
{
    public class WhenICreateAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private EmployerAccountData _accountData;
        private OrchestratorResponse<EmployerAgreementViewModel> _response;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<ICookieStorageService<ReturnUrlModel>> _returnUrlCookieStorage;
        private const string HashedAccountId = "ABC123";
        private const string ExpectedReturnUrl = "test.com";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _returnUrlCookieStorage = new Mock<ICookieStorageService<ReturnUrlModel>>();

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

            _orchestrator.Setup(x => x.CreateOrUpdateAccount(It.IsAny<CreateAccountModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);
            
            _returnUrlCookieStorage.Setup(x => x.Get("SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie"))
                .Returns(new ReturnUrlModel {Value = ExpectedReturnUrl});

            _employerAccountController = new EmployerAccountController(
                Mock.Of<IAuthenticationService>(),
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                _returnUrlCookieStorage.Object,
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenIShouldGoToTheReturnUrl()
        {
            //Act
            var result = await _employerAccountController.CreateAccount() as Microsoft.AspNetCore.Mvc.RedirectResult;

            //Assert
            Assert.AreEqual(ExpectedReturnUrl, result.Url);
        }
    }
}
