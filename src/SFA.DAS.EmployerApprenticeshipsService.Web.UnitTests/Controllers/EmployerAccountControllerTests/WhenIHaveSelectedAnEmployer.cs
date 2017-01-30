using System;
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
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenIHaveSelectedAnEmployer : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private EmployerAccountData _accountData;
        private OrchestratorResponse<EmployerAgreementViewModel> _response;
        private const string hashedAccountId = "ABC123";

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
                OrganisationReferenceNumber = "1244454",
                OrganisationRegisteredAddress = "1, Test Street",
                OrganisationDateOfInception = DateTime.Now.AddYears(-10)
            };

            _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>()))
                       .Returns(_accountData);

            _response = new OrchestratorResponse<EmployerAgreementViewModel>()
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        HashedAccountId = hashedAccountId
                    }
                },
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x => x.CreateAccount(It.IsAny<CreateAccountViewModel>(), It.IsAny<HttpContextBase>()))
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
