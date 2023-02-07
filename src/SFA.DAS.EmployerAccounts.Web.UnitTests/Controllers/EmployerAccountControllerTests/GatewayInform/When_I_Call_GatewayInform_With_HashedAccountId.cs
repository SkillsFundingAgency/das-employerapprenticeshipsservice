using System.Diagnostics.CodeAnalysis;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GatewayInform;

[ExcludeFromCodeCoverage]
public class When_I_Call_GatewayInform_With_HashedAccountId : ControllerTestBase
{
    private EmployerAccountController _employerAccountController;
    private Mock<EmployerAccountOrchestrator> _orchestrator;
    private Mock<IAuthenticationService> _owinWrapper;     
    private string _hashedAccountId = @"W4X7DL";
    private Mock<ICookieStorageService<HashedAccountIdModel>> _mockAccountCookieStorage;
    private string _cookieKeyName;

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        _cookieKeyName = typeof(HashedAccountIdModel).FullName;

        _orchestrator = new Mock<EmployerAccountOrchestrator>();

        _owinWrapper = new Mock<IAuthenticationService>();        

        _orchestrator.Setup(x => x.RenameEmployerAccount(It.IsAny<RenameEmployerAccountViewModel>(), It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<RenameEmployerAccountViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = new RenameEmployerAccountViewModel()
            });

        _mockAccountCookieStorage = new Mock<ICookieStorageService<HashedAccountIdModel>>();

        _employerAccountController = new EmployerAccountController(
            Mock.Of<EmployerAccountOrchestrator>(),
            Mock.Of<ILogger<EmployerAccountController>>(),
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            _mockAccountCookieStorage.Object,
            Mock.Of<IHttpContextAccessor>())
        {
            ControllerContext = ControllerContext.Object,
            Url = new UrlHelper(new ActionContext(HttpContext.Object, Routes, new ActionDescriptor()))
        };
    }

    [Test]
    public void Then_The_HashedAccountId_Is_Stored()
    {
        _employerAccountController.GatewayInform(hashedAccountId: _hashedAccountId);

        _mockAccountCookieStorage
            .Verify(
                m =>
                    m.Delete(_cookieKeyName));

        _mockAccountCookieStorage
            .Verify(
                m => m.Create(
                    It.Is<HashedAccountIdModel>(model => model.Value.Equals(_hashedAccountId)),
                    It.Is<string>(key => key.Equals(_cookieKeyName)),
                    It.IsAny<int>())
            );
    }

}