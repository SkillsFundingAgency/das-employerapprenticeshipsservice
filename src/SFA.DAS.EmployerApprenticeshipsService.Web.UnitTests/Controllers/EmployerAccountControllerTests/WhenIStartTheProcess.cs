using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountControllerTests
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
        public void ThenIfThePayeSchemeIsInUseMySearchedCompanyDetailsThatAreSavedAreUsed()
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
            _employerAccountController.GatewayInform(new SelectEmployerViewModel());

            //Assert
            _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.Is<object>(
                c =>  ((EmployerAccountData)c).CompanyName.Equals(companyName) 
                && ((EmployerAccountData)c).CompanyNumber.Equals(companyNumber) 
                && ((EmployerAccountData)c).DateOfIncorporation.Equals(dateOfIncorporation) 
                && ((EmployerAccountData)c).RegisteredAddress.Equals(registeredAddress)
                )));

        }

        [Test]
        public void ThenIfTheCompanyInformationModelIsNotEmptyTheDataIsNotReadFromTheCookie()
        {
            //Act
            var companyName = "Test";
            var companyNumber = "123TEST";
            var registeredAddress = "Test Address";
            var dateOfIncorporation = new DateTime(2016, 05, 25);
            _employerAccountController.GatewayInform(new SelectEmployerViewModel
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
        public async Task ThenIAmRedirectedToTheGovermentGatewayWhenIConfirmIHaveGatewayCredentials()
        {
            //Arrange
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);
            _orchestrator.Setup(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.IsAny<EmployerAccountData>()));

            //Act
            var actual = await _employerAccountController.Gateway();

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as RedirectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(ExpectedRedirectUrl, actualResult.Url);
        }
            

        [Test]
        public void ThenTheCookieIsDeletedWhenIStartTheAddAccountProcess()
        {
            //Act
            _employerAccountController.SelectEmployer();

            //Assert
            _orchestrator.Verify(x => x.DeleteCookieData(It.IsAny<HttpContextBase>()), Times.Once);
        }
        
    }
}
