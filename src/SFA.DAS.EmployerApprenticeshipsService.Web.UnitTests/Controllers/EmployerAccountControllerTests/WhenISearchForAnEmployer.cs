using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenISearchForAnEmployer : ControllerTestBase
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
        }

        [Test]
        public async Task ThenItCannotBeFoundAtCompaniesHouse()
        {
            //Assign
            _orchestrator.Setup(x => x.GetCompanyDetails(It.IsAny<SelectEmployerModel>())).ReturnsAsync(new OrchestratorResponse<SelectEmployerViewModel>()
            {
                Status = HttpStatusCode.BadRequest
            });

            //Act
            var result = await _employerAccountController.SelectEmployer(new SelectEmployerModel()) as ViewResult;
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.TempData.ContainsKey("companyNumberError"));
        }
    }
}
