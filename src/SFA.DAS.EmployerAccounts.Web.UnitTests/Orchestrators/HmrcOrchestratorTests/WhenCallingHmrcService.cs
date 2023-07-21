using HMRC.ESFA.Levy.Api.Types;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.HmrcOrchestratorTests;

public class WhenCallingHmrcService
{
    private EmployerAccountOrchestrator _employerAccountOrchestrator;
    private Mock<ILogger<EmployerAccountOrchestrator>> _logger;
    private Mock<IMediator> _mediator;
    private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

    private EmployerAccountsConfiguration _configuration;

    [SetUp]
    public void Arrange()
    {
        _logger = new Mock<ILogger<EmployerAccountOrchestrator>>();
        _mediator = new Mock<IMediator>();
        _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            
        _configuration = new EmployerAccountsConfiguration
        {
            Hmrc = new HmrcConfiguration()
        };

        _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration, Mock.Of<IEncodingService>());   
    }

    [Test]
    public async Task ThenTheHmrcServiceIsCalled()
    {
        //Arrange
        var redirectUrl = "myUrl";
        _mediator.Setup(x => x.Send(It.IsAny<GetGatewayInformationQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetGatewayInformationResponse { Url = "someurl" });

        //Act
        await _employerAccountOrchestrator.GetGatewayUrl(redirectUrl);

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetGatewayInformationQuery>(c => c.ReturnUrl.Equals(redirectUrl)), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task ThenICanRetrieveEmployerInformationWithMyAccessToken()
    {
        //Arrange
        var expectedAuthToken = "123";
        _mediator.Setup(x => x.Send(It.IsAny<GetHmrcEmployerInformationQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation() });

        //Act
        await _employerAccountOrchestrator.GetHmrcEmployerInformation(expectedAuthToken, string.Empty);

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetHmrcEmployerInformationQuery>(c => c.AuthToken.Equals(expectedAuthToken)), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task ThenICanRetrieveCorrectEmpRefOfScenarioUser()
    {
        //Arrange
        var scenarioUserEmail = "test.user@test.com";
        var expectedEmpRef = "123/456789";

        _mediator.Setup(x => x.Send(It.IsAny<GetHmrcEmployerInformationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetHmrcEmployerInformationResponse { EmployerLevyInformation = new EmpRefLevyInformation(), Empref = expectedEmpRef });
          
        //Act
        var result = await _employerAccountOrchestrator.GetHmrcEmployerInformation("123", scenarioUserEmail);
            
        //Assert
        Assert.AreEqual(expectedEmpRef, result.Empref);
    }

    [Test]
    public async Task ThenIfANotFoundExceptionIsThrownThePropertyIsSetOnTheResponse()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<GetHmrcEmployerInformationQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException("Empref not found"));

        //Act
        var result = await _employerAccountOrchestrator.GetHmrcEmployerInformation("123", "test@test.com");

        //Assert
        Assert.IsTrue(result.EmprefNotFound);
    }
}