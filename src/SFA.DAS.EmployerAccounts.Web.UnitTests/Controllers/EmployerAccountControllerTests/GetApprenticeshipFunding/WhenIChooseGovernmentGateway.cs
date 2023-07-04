using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GetApprenticeshipFunding;

class WhenIChooseGovernmentGateway : ControllerTestBase
{
    private EmployerAccountController _employerAccountController;
    private Mock<EmployerAccountOrchestrator> _orchestrator;
    private const string ExpectedRedirectUrl = "http://redirect.local.test";
    private OrchestratorResponse<EmployerAccountViewModel> _response;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private const string HashedAccountId = "ABC123";
    private const string ExpectedUserId = "USER123";

    [SetUp]
    public void Arrange()
    {
        base.Arrange(ExpectedRedirectUrl);

        _orchestrator = new Mock<EmployerAccountOrchestrator>();

        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _response = new OrchestratorResponse<EmployerAccountViewModel>()
        {
            Data = new EmployerAccountViewModel
            {
                HashedId = HashedAccountId
            },
            Status = HttpStatusCode.OK
        };

        _orchestrator.Setup(x => x.CreateMinimalUserAccountForSkipJourney(It.IsAny<CreateUserAccountViewModel>(), It.IsAny<HttpContext>()))
            .ReturnsAsync(_response);

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
    public void ThenIShouldGoToGatewayInform()
    {
        //Act
        var result = _employerAccountController.GetApprenticeshipFunding(string.Empty, 2) as RedirectToActionResult;

        //Assert
        Assert.AreEqual(ControllerConstants.GatewayInformActionName, result.ActionName);
    }
}