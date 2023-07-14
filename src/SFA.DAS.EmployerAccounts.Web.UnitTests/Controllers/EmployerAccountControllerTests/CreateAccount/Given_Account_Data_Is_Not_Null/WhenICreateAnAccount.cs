using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.CreateAccount.Given_Cookie_Data_Is_Not_Null;

class WhenICreateAnAccount : ControllerTestBase
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

        var logger = new Mock<ILogger<EmployerAccountController>>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();


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

        AddUserToContext();

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
       
        _employerAccountController = new EmployerAccountController(
            _orchestrator.Object,
            logger.Object,
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
    public async Task ThenIShouldGoToWhenDoYouWantToView()
    {
        //Act
        var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

        //Assert
        result.RouteName.Should().Be(RouteNames.OrganisationAndPayeAddedSuccess);
    }

    [Test]
    public async Task ThenIShouldGetBackTheAccountId()
    {
        //Act
        var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HashedAccountId, result.RouteValues["HashedAccountId"]);
    }
        
    [Test]
    public async Task ThenTheParamtersArePassedFromTheCookieWhenCreatingTheAccount()
    {
        //Act
        await _employerAccountController.CreateAccount();

        //Assert
        _orchestrator.Verify(x => x.CreateOrUpdateAccount(It.Is<CreateAccountModel>(
            c =>
                c.OrganisationStatus.Equals(_accountData.EmployerAccountOrganisationData.OrganisationStatus) &&
                c.OrganisationName.Equals(_accountData.EmployerAccountOrganisationData.OrganisationName) &&
                c.RefreshToken.Equals(_accountData.EmployerAccountPayeRefData.RefreshToken) &&
                c.OrganisationDateOfInception.Equals(_accountData.EmployerAccountOrganisationData.OrganisationDateOfInception) &&
                c.OrganisationAddress.Equals(_accountData.EmployerAccountOrganisationData.OrganisationRegisteredAddress) &&
                c.AccessToken.Equals(_accountData.EmployerAccountPayeRefData.AccessToken) &&
                c.PayeReference.Equals(_accountData.EmployerAccountPayeRefData.PayeReference) &&
                c.EmployerRefName.Equals(_accountData.EmployerAccountPayeRefData.EmployerRefName) &&
                c.Sector.Equals(_accountData.EmployerAccountOrganisationData.Sector) &&
                c.OrganisationReferenceNumber.Equals(_accountData.EmployerAccountOrganisationData.OrganisationReferenceNumber)
        ), It.IsAny<HttpContext>()));
    }
}