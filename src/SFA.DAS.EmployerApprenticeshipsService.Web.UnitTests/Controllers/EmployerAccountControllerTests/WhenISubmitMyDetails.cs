using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
    public class WhenISubmitMyDetails : ControllerTestBase
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

            _employerAccountController = new EmployerAccountController(
               _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userWhiteList.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };

            _accountData = new EmployerAccountData
            {
                CompanyName = "Test Corp",
                CompanyNumber = "1244454",
                RegisteredAddress = "1, Test Street",
                DateOfIncorporation = DateTime.Now.AddYears(-10)
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

            _orchestrator.Setup(x => x.CreateAccount(It.IsAny<CreateAccountModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);
        }

        [Test]
        public void ThenTheInformationIsReadFromTheCookie()
        {
            //Arrange
            var employerAccountData = new EmployerAccountData
            {
                CompanyStatus = "Active",
                CompanyName = "Test Company",
                DateOfIncorporation = DateTime.MaxValue,
                CompanyNumber = "ABC12345",
                RegisteredAddress = "My Address",
                EmployerRef = "123/abc",
                EmpRefNotFound = true
            };
            _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>())).Returns(employerAccountData);


            //Act
            var actual = _employerAccountController.Summary();

            //Assert
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as SummaryViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(employerAccountData.CompanyName, model.CompanyName);
            Assert.AreEqual(employerAccountData.CompanyStatus, model.CompanyStatus);
            Assert.AreEqual(employerAccountData.CompanyNumber, model.CompanyNumber);
            Assert.AreEqual(employerAccountData.EmployerRef, model.EmployerRef);
            Assert.AreEqual(employerAccountData.EmpRefNotFound, model.EmpRefNotFound);

        }
    }
}
