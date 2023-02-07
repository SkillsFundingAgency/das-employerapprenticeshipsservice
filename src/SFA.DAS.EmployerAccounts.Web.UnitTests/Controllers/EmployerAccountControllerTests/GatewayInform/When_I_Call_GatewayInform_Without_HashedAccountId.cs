using System.Diagnostics.CodeAnalysis;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GatewayInform;

[ExcludeFromCodeCoverage]
public class When_I_Call_GatewayInform_Without_HashedAccountId : ControllerTestBase
{
    private EmployerAccountController _employerAccountController;
    private Mock<EmployerAccountOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<HashedAccountIdModel>> _mockAccountCookieStorage;
        
    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        _orchestrator = new Mock<EmployerAccountOrchestrator>();
            
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
            Mock.Of<LinkGenerator>())
        {
            ControllerContext = ControllerContext,
            Url = new UrlHelper(new ActionContext(HttpContext.Object, Routes, new ActionDescriptor()))
        };
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("                        ")]
    public void Then_The_HashedAccountId_Is_Not_Stored(string hashedAccountId)
    {
        _employerAccountController.GatewayInform(hashedAccountId: hashedAccountId);

        _mockAccountCookieStorage
            .Verify(
                m =>
                    m.Delete(It.IsAny<string>()),
                Times.Never());

        _mockAccountCookieStorage
            .Verify(
                m => m.Create(
                    It.IsAny<HashedAccountIdModel>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()),
                Times.Never
            );
    }
}