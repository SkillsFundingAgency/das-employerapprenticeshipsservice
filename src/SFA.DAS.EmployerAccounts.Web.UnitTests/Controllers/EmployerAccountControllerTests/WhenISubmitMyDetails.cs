using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests;

public class WhenISubmitMyDetails : ControllerTestBase
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

    [Test]
    public void ThenIAmShownASummary()
    {
        //Arrange
        _orchestrator.Setup(x => x.GetSummaryViewModel(It.IsAny<HttpContext>()))
            .Returns(new OrchestratorResponse<SummaryViewModel>());

        //Act
        var actual = _employerAccountController.Summary();

        //Assert
        _orchestrator.Verify(x=> x.GetSummaryViewModel(It.IsAny<HttpContext>()), Times.Once);
        Assert.IsNotNull(actual);
        var model = actual.Model as OrchestratorResponse<SummaryViewModel>;
        Assert.IsNotNull(model);
    }
}