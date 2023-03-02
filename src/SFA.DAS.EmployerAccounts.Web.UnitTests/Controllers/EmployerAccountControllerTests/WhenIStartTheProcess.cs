using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests;

public class WhenIStartTheProcess : ControllerTestBase
{
    private EmployerAccountController _employerAccountController;
    private Mock<EmployerAccountOrchestrator> _orchestrator;
    private readonly string _expectedRedirectUrl = "http://redirect.local.test";
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;


    [SetUp]
    public void Arrange()
    {
        base.Arrange(_expectedRedirectUrl);
            
        _orchestrator = new Mock<EmployerAccountOrchestrator>();
        _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(_expectedRedirectUrl);

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
    }
    [Test]
    public async Task ThenIAmRedirectedToTheGovernmentGatewayWhenIConfirmIHaveGatewayCredentials()
    {
        //Arrange
        _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(_expectedRedirectUrl);

        //Act
        var actual = await _employerAccountController.Gateway();

        //Assert
        Assert.IsNotNull(actual);
        var actualResult = actual as RedirectResult;
        Assert.IsNotNull(actualResult);
        Assert.AreEqual(_expectedRedirectUrl, actualResult.Url);
        actualResult.Url.Should().NotBeNull();
        actualResult.Url.Should().Be(_expectedRedirectUrl);
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
}