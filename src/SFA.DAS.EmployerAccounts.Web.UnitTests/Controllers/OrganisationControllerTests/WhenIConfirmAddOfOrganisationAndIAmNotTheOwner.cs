using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

public class WhenIConfirmAddOfOrganisationAndIAmNotTheOwner : ControllerTestBase
{
    private OrganisationController _controller;
    private readonly Mock<OrganisationOrchestrator> _orchestrator = new();
    private readonly Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage = new();

    [SetUp]
    public void Arrange()
    {
        base.Arrange();
        AddUserToContext();

        _orchestrator.Setup(x => x.CreateLegalEntity(It.IsAny<CreateNewLegalEntityViewModel>()))
            .Throws<UnauthorizedAccessException>();

        _controller = new OrganisationController(
            _orchestrator.Object,
            _flashMessage.Object,
            Mock.Of<IMultiVariantTestingService>())
        {
            ControllerContext = ControllerContext
        };
    }

    [Test]
    public async Task ThenIAmRedirectedToAccessDenied()
    {
        //Act & Asert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, false));
    }
}