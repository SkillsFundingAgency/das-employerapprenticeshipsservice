using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

public class WhenIConfirmAddOfOrganisationAndIAmNotTheOwner : ControllerTestBase
{
    private OrganisationController _controller;
    private Mock<OrganisationOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {
        base.Arrange();
        AddUserToContext();

        _orchestrator = new Mock<OrganisationOrchestrator>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _orchestrator.Setup(x => x.CreateLegalEntity(It.IsAny<CreateNewLegalEntityViewModel>()))
            .Throws<UnauthorizedAccessException>();

        _controller = new OrganisationController(
            _orchestrator.Object,
            _flashMessage.Object)
        {
            ControllerContext = ControllerContext
        };
    }

    [Test]
    public void ThenIAmRedirectedToAccessDenied()
    {
        //Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, false));
    }
}