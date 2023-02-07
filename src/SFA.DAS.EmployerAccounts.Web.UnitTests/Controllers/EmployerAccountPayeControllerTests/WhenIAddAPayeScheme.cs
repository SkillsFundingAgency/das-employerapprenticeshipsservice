using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountPayeControllerTests;

public class WhenIAddAPayeScheme
{
    private Mock<EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
    private Mock<IAuthenticationService> _owinWrapper;
    private EmployerAccountPayeController _controller;
    private Mock<IMultiVariantTestingService> _userViewTestingService;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
    private const string ExpectedAccountId = "AFD123";
    private const string ExpectedUserId = "456TGF3";

    [SetUp]
    public void Arrange()
    {
        _employerAccountPayeOrchestrator = new Mock<EmployerAccountPayeOrchestrator>();
            
        _owinWrapper = new Mock<IAuthenticationService>();
        _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);
        _userViewTestingService = new Mock<IMultiVariantTestingService>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _controller = new EmployerAccountPayeController(
            _employerAccountPayeOrchestrator.Object, 
            _flashMessage.Object);
    }

    [Test]
    public async Task ThenTheAddPayeSchemeToAccountIsCalledWithTheCorrectParameters()
    {
        //Arrange
        var expectedAddNewPayeScheme = new AddNewPayeSchemeViewModel {AccessToken = "123DFG",HashedAccountId = ExpectedAccountId,PayeName = "123/ABC",RefreshToken = "987TGH"};

        _employerAccountPayeOrchestrator.Setup(
                x => x.AddPayeSchemeToAccount(It.IsAny<AddNewPayeSchemeViewModel>(), It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<AddNewPayeSchemeViewModel>
            {
                Status = HttpStatusCode.OK
            });
                
        //Act
        await _controller.ConfirmPayeScheme(ExpectedAccountId, expectedAddNewPayeScheme);

        //Assert
        _employerAccountPayeOrchestrator.Verify(x=>x.AddPayeSchemeToAccount(It.Is<AddNewPayeSchemeViewModel>(
            c=>c.AccessToken.Equals(expectedAddNewPayeScheme.AccessToken) &&
               c.HashedAccountId.Equals(expectedAddNewPayeScheme.HashedAccountId) &&
               c.PayeName.Equals(expectedAddNewPayeScheme.PayeName) &&
               c.RefreshToken.Equals(expectedAddNewPayeScheme.RefreshToken)
        ),ExpectedUserId));
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
        _flashMessage.Verify(x=>x.Create(It.Is<FlashMessageViewModel>(c=>c.HiddenFlashMessageInformation.Equals("page-paye-scheme-added")),It.IsAny<string>(), 1));
    }
}