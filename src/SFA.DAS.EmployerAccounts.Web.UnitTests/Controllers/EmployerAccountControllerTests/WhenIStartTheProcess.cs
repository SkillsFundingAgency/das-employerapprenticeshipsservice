using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private string ExpectedRedirectUrl = "http://redirect.local.test";
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;


        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);
            
            _orchestrator = new Mock<EmployerAccountOrchestrator>();
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);

            _owinWrapper = new Mock<IAuthenticationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

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
        }

        //TODO add EmployerAccountOrganisationController tests when created
        //[Test]
        //public void ThenIfThePayeSchemeIsInUseMySearchedCompanyDetailsThatAreSavedAreUsed()
        //{
        //    //Arrange
        //    var companyName = "My Company";
        //    var companyNumber = "12345";
        //    var dateOfIncorporation = new DateTime(2016, 01, 10);
        //    var registeredAddress = "Test Address";
        //    _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>())).Returns(new EmployerAccountData
        //    {
        //        OrganisationName = companyName,
        //        OrganisationReferenceNumber = companyNumber,
        //        OrganisationDateOfInception = dateOfIncorporation,
        //        OrganisationRegisteredAddress = registeredAddress,

        //    });


        //    //Act
        //    _employerAccountController.GatewayInform(new OrganisationDetailsViewModel());

        //    //Assert
        //    _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.Is<object>(
        //        c =>  ((EmployerAccountData)c).OrganisationName.Equals(companyName) 
        //        && ((EmployerAccountData)c).OrganisationReferenceNumber.Equals(companyNumber) 
        //        && ((EmployerAccountData)c).OrganisationDateOfInception.Equals(dateOfIncorporation) 
        //        && ((EmployerAccountData)c).OrganisationRegisteredAddress.Equals(registeredAddress)
        //        )));

        //}

        //[Test]
        //public void ThenIfTheCompanyInformationModelIsNotEmptyTheDataIsNotReadFromTheCookie()
        //{
        //    //Act
        //    var companyName = "Test";
        //    var companyNumber = "123TEST";
        //    var registeredAddress = "Test Address";
        //    var dateOfIncorporation = new DateTime(2016, 05, 25);
        //    _employerAccountController.GatewayInform(new OrganisationDetailsViewModel
        //    {
        //        Name = companyName,
        //        ReferenceNumber = companyNumber,
        //        Address = registeredAddress,
        //        DateOfInception = dateOfIncorporation
        //    });

        //    //Assert
        //    _orchestrator.Verify(x=>x.GetCookieData(It.IsAny<HttpContextBase>()), Times.Never);
        //    _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.Is<object>(
        //        c => ((EmployerAccountData)c).OrganisationName.Equals(companyName)
        //        && ((EmployerAccountData)c).OrganisationReferenceNumber.Equals(companyNumber)
        //        && ((EmployerAccountData)c).OrganisationDateOfInception.Equals(dateOfIncorporation)
        //        && ((EmployerAccountData)c).OrganisationRegisteredAddress.Equals(registeredAddress)
        //        )));
        //}

        [Test]
        public async Task ThenIAmRedirectedToTheGovernmentGatewayWhenIConfirmIHaveGatewayCredentials()
        {
            //Arrange
            _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);

            //Act
            var actual = await _employerAccountController.Gateway();

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as RedirectResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(ExpectedRedirectUrl, actualResult.Url);
        }
    }
}
