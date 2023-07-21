using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests;

public class WhenIAddAPayeScheme : ControllerTestBase
{
    private Mock<EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
    private EmployerAccountPayeController _controller;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private const string ExpectedAccountId = "AFD123";
    private const string ExpectedUserId = "456TGF3";

    [SetUp]
    public void Arrange()
    {
        base.Arrange();

        AddUserToContext(ExpectedUserId);

        _employerAccountPayeOrchestrator = new Mock<EmployerAccountPayeOrchestrator>();

        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _controller = new EmployerAccountPayeController(
            Mock.Of<IUrlActionHelper>(),
            _employerAccountPayeOrchestrator.Object,
            _flashMessage.Object,
            Mock.Of<LinkGenerator>())
        {
            ControllerContext = ControllerContext
        };
    }

    [Test]
    public async Task ThenTheAddPayeSchemeToAccountIsCalledWithTheCorrectParameters()
    {
        //Arrange
        var expectedAddNewPayeScheme = new AddNewPayeSchemeViewModel { AccessToken = "123DFG", HashedAccountId = ExpectedAccountId, PayeName = "123/ABC", RefreshToken = "987TGH" };

        _employerAccountPayeOrchestrator.Setup(
                x => x.AddPayeSchemeToAccount(It.Is<AddNewPayeSchemeViewModel>(y => y == expectedAddNewPayeScheme), It.Is<string>(s => s == ExpectedUserId)))
            .ReturnsAsync(new OrchestratorResponse<AddNewPayeSchemeViewModel>
            {
                Status = HttpStatusCode.OK
            });

        //Act
        await _controller.ConfirmPayeScheme(ExpectedAccountId, expectedAddNewPayeScheme);

        //Assert
        _employerAccountPayeOrchestrator.Verify(x => x.AddPayeSchemeToAccount(It.Is<AddNewPayeSchemeViewModel>(y => y == expectedAddNewPayeScheme), It.Is<string>(s => s == ExpectedUserId)), Times.Once);
    }

    [Test]
    public async Task ThenTheSuccessMessageIsCorrectlyPopulated()
    {
        //Arrange
        _employerAccountPayeOrchestrator.Setup(
                x => x.AddPayeSchemeToAccount(It.IsAny<AddNewPayeSchemeViewModel>(), It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<AddNewPayeSchemeViewModel>
            {
                Status = HttpStatusCode.OK
            });

        //Act
        await _controller.ConfirmPayeScheme(ExpectedAccountId, new AddNewPayeSchemeViewModel());

        //Assert
        _flashMessage.Verify(x => x.Create(It.Is<FlashMessageViewModel>(c => c.HiddenFlashMessageInformation.Equals("page-paye-scheme-added")), It.IsAny<string>(), 1));
    }
}