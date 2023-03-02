using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenIHaveSelectedAnEmployer : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
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

            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _employerAccountController = new EmployerAccountController(
                _orchestrator.Object,
                Mock.Of<ILogger<EmployerAccountController>>(),
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>(),
                Mock.Of<LinkGenerator>())
            {
                ControllerContext = ControllerContext,
                Url = new UrlHelper(new ActionContext(MockHttpContext.Object, Routes, new ActionDescriptor()))
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

            _orchestrator.Setup(x => x.CreateOrUpdateAccount(It.IsAny<CreateAccountModel>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(_response);
        }

        //TODO add EmployerAccountOrganisationController tests when created
        //[Test]
        //public void ThenIShouldSaveTheSelectedEmployerDetailsToCookies()
        //{
        //    //Assign
        //    var request = new OrganisationDetailsViewModel
        //    {
        //        ReferenceNumber = "6576585",
        //        Name = "Test Corp",
        //        DateOfInception = DateTime.Now.AddYears(-12),
        //        Address = "1, Test Street",
        //        Status = "active"
        //    };

        //    _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);
        //    _orchestrator.Setup(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.IsAny<EmployerAccountData>()));

        //    //Act
        //    _employerAccountController.GatewayInform(request);

        //    //Assert
        //    _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(),
        //        It.Is<EmployerAccountData>(data => 
        //        data.OrganisationReferenceNumber.Equals(request.ReferenceNumber) &&
        //        data.OrganisationStatus.Equals(request.Status) &&
        //        data.OrganisationName.Equals(request.Name) &&
        //        data.OrganisationDateOfInception.Equals(request.DateOfInception) &&
        //        data.OrganisationRegisteredAddress.Equals(request.Address))));
        //}

        //[Test]
        //public void ThenTheDataWillBeReadFromTheCookieIfIAmGoingBackThroughTheProcess()
        //{
        //    //Arrange
        //    var request = new EmployerAccountData
        //    {
        //        OrganisationReferenceNumber = "6576585",
        //        OrganisationName = "Test Corp",
        //        OrganisationDateOfInception = DateTime.Now.AddYears(-12),
        //        OrganisationRegisteredAddress = "1, Test Street",
        //        OrganisationStatus = "active"
        //    };
        //    _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>())).Returns(request);

        //    //Act
        //    _employerAccountController.GatewayInform(null);

        //    //Assert
        //    _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(),
        //        It.Is<EmployerAccountData>(data =>
        //        data.OrganisationReferenceNumber.Equals(request.OrganisationReferenceNumber) &&
        //        data.OrganisationStatus.Equals(request.OrganisationStatus) &&
        //        data.OrganisationName.Equals(request.OrganisationName) &&
        //        data.OrganisationDateOfInception.Equals(request.OrganisationDateOfInception) &&
        //        data.OrganisationRegisteredAddress.Equals(request.OrganisationRegisteredAddress))));
        //}
    }
}
