using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

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
            
        _orchestrator.Setup(x => x.RenameEmployerAccount(It.IsAny<string>(), It.IsAny<RenameEmployerAccountViewModel>(), It.IsAny<string>()))
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
            Url = new UrlHelper(new ActionContext(MockHttpContext.Object, Routes, new ActionDescriptor()))
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

    [Test]
    [MoqAutoData]
    public void When_Within_Account_Then_Cancel_Returns_To_Paye_Index(string hashedAccountId)
    {
        // Act
        var result = _employerAccountController.GatewayInform(hashedAccountId: hashedAccountId) as ViewResult;
        var model = result.Model as OrchestratorResponse<GatewayInformViewModel>;

        // Assert
        model.Data.CancelRoute.Should().Be(RouteNames.EmployerAccountPaye);
    }

    [Test]
    [MoqAutoData]
    public void When_Within_Account_Then_Cancel_Returns_To_Task_List(string hashedAccountId)
    {
        // Act
        var result = _employerAccountController.GatewayInform(hashedAccountId: null) as ViewResult;
        var model = result.Model as OrchestratorResponse<GatewayInformViewModel>;

        // Assert
        model.Data.CancelRoute.Should().Be(RouteNames.NewEmpoyerAccountTaskList);
    }
}