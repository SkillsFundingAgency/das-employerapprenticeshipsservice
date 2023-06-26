using MediatR;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.RenameAccount;

public class WhenIRenameAnAccount : ControllerTestBase
{
    private EmployerAccountController _employerAccountController;
    private Mock<EmployerAccountOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private const string ExpectedRedirectUrl = "http://redirect.local.test";

    [SetUp]
    public void Arrange()
    {
        base.Arrange(ExpectedRedirectUrl);

        _orchestrator = new Mock<EmployerAccountOrchestrator>();

        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _orchestrator.Setup(x =>
                x.RenameEmployerAccount(It.IsAny<string>(), It.IsAny<RenameEmployerAccountViewModel>(), It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<RenameEmployerAccountViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = new RenameEmployerAccountViewModel()
            });


        AddUserToContext();

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

    [Test, MoqAutoData]
    public async Task ThenTheAccountIsRenamed(string hashedAccountId)
    {
        //Arrange
        var model = new RenameEmployerAccountViewModel
        {
            CurrentName = "Test Account",
            NewName = "New Account Name"
        };

        //Act
        await _employerAccountController.RenameAccount(hashedAccountId, model);

        //Assert
        _orchestrator.Verify(x => x.RenameEmployerAccount(hashedAccountId, It.Is<RenameEmployerAccountViewModel>(r =>
            r.CurrentName == "Test Account"
            && r.NewName == "New Account Name"
        ), It.IsAny<string>()));
    }
}