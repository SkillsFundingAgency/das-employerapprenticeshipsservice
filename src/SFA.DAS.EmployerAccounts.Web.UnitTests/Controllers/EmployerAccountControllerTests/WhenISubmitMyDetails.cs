using System;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenISubmitMyDetails : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;      
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
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            var logger = new Mock<ILog>();

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };

            _accountData = new EmployerAccountData
            {
                EmployerAccountOrganisationData = new EmployerAccountOrganisationData
                { 
                OrganisationName = "Test Corp",
                OrganisationReferenceNumber = "1244454",
                OrganisationRegisteredAddress = "1, Test Street",
                OrganisationDateOfInception = DateTime.Now.AddYears(-10)
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
        }

        [Test]
        public void ThenIAmShownASummary()
        {
            //Arrange
            _orchestrator.Setup(x => x.GetSummaryViewModel(It.IsAny<HttpContextBase>()))
                .Returns(new OrchestratorResponse<SummaryViewModel>());

            //Act
            var actual = _employerAccountController.Summary();

            //Assert
            _orchestrator.Verify(x=> x.GetSummaryViewModel(It.IsAny<HttpContextBase>()), Times.Once);
            Assert.IsNotNull(actual);
            var model = actual.Model as OrchestratorResponse<SummaryViewModel>;
            Assert.IsNotNull(model);
        }
    }
}
