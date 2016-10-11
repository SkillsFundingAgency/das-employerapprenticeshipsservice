using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;
        private string ExpectedRedirectUrl = "http://redirect.local.test";


        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);
            
            _orchestrator = new Mock<EmployerAccountOrchestrator>();
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);

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
        public void ThenIAmPresentedWithTheEligibilityPage()
        {
            //Act
            var actual = _employerAccountController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(string.Empty, actualViewResult.ViewName);
        }

        [Test]
        public async Task ThenIfThePayeSchemeIsInUseMySearchedCompanyDetailsThatAreSavedAreUsed()
        {
            //Arrange
            var companyName = "My Company";
            var companyNumber = "12345";
            var dateOfIncorporation = new DateTime(2016, 01, 10);
            var registeredAddress = "Test Address";
            _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>())).Returns(new EmployerAccountData
            {
                CompanyName = companyName,
                CompanyNumber = companyNumber,
                DateOfIncorporation = dateOfIncorporation,
                RegisteredAddress = registeredAddress
            });
            

            //Act
            await _employerAccountController.Gateway(new SelectEmployerViewModel());

            //Assert
            _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.Is<object>(
                c =>  ((EmployerAccountData)c).CompanyName.Equals(companyName) 
                && ((EmployerAccountData)c).CompanyNumber.Equals(companyNumber) 
                && ((EmployerAccountData)c).DateOfIncorporation.Equals(dateOfIncorporation) 
                && ((EmployerAccountData)c).RegisteredAddress.Equals(registeredAddress)
                )));

        }

        [Test]
        public async Task ThenIfTheCompanyInformationModelIsNotEmptyTheDataIsNotReadFromTheCookie()
        {
            //Act
            var companyName = "Test";
            var companyNumber = "123TEST";
            var registeredAddress = "Test Address";
            var dateOfIncorporation = new DateTime(2016, 05, 25);
            await _employerAccountController.Gateway(new SelectEmployerViewModel
            {
                CompanyName = companyName,
                CompanyNumber = companyNumber,
                RegisteredAddress = registeredAddress,
                DateOfIncorporation = dateOfIncorporation
            });

            //Assert
            _orchestrator.Verify(x=>x.GetCookieData(It.IsAny<HttpContextBase>()), Times.Never);
            _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.Is<object>(
                c => ((EmployerAccountData)c).CompanyName.Equals(companyName)
                && ((EmployerAccountData)c).CompanyNumber.Equals(companyNumber)
                && ((EmployerAccountData)c).DateOfIncorporation.Equals(dateOfIncorporation)
                && ((EmployerAccountData)c).RegisteredAddress.Equals(registeredAddress)
                )));
        }

        [Test]
        public void ThenICanProceedToTheGovernmentGatewayConfirmationPage()
        {
            //Act
            var actual = _employerAccountController.Index("understood");

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("SelectEmployer", actualRedirectResult.RouteValues["Action"]);
        }

        [Test]
        public async Task ThenIAmRedirectedToTheGovermentGatewayWhenIConfirmIHaveGatewayCredentials()
        {
            //Arrange
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);
            _orchestrator.Setup(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.IsAny<EmployerAccountData>()));

            //Act
            var actual = await _employerAccountController.Gateway(new SelectEmployerViewModel {CompanyName = "Test"});

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as RedirectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(ExpectedRedirectUrl, actualResult.Url);
        }

        [Test]
        [Ignore("Cant test this without some serious refactoring of the Controller")]
        public async Task ThenTheAccessCodeIsTakenFromTheUrlExchangedForThe()
        {
            //Arrange

            //_mediator.Setup(x => x.SendAsync(It.IsAny<GetGatewayTokenQuery>())).ReturnsAsync(new GetGatewayTokenQueryResponse() { HmrcTokenResponse = new HmrcTokenResponse()});

            //Act
            var actual = await _employerAccountController.GateWayResponse();
        }

        [Test]
        public void ThenTheCookieIsDeletedWhenIStartTheAddAccountProcess()
        {
            //Act
            _employerAccountController.Index();

            //Assert
            _orchestrator.Verify(x => x.DeleteCookieData(It.IsAny<HttpContextBase>()), Times.Once);
        }
    }
}
