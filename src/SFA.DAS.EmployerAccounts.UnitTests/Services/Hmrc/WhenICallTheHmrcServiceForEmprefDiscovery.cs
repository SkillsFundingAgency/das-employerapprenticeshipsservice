using System.Collections.Generic;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.UnitTests.Policies.Hmrc;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Hmrc;

public class WhenICallTheHmrcServiceForEmprefDiscovery
{
    private const string ExpectedEmpref = "123/AB12345";
    private const string ExpectedAuthToken = "789654321AGFVD";
    private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
    private const string ExpectedClientId = "654321";
    private const string ExpectedClientSecret = "my_secret";
    private const string ExpectedScope = "emp_ref";
    private Mock<IApprenticeshipLevyApiClient> _apprenticeshipLevyApiClient;
    private HmrcConfiguration _configuration;

    private HmrcService _hmrcService;
    private Mock<IHttpClientWrapper> _httpClientWrapper;
    private Mock<ITokenServiceApiClient> _tokenService;

    [SetUp]
    public void Arrange()
    {
        _configuration = new HmrcConfiguration
        {
            BaseUrl = ExpectedBaseUrl,
            ClientId = ExpectedClientId,
            Scope = ExpectedScope,
            ClientSecret = ExpectedClientSecret
        };

        _httpClientWrapper = new Mock<IHttpClientWrapper>();

        _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();
        _apprenticeshipLevyApiClient.Setup(x => x.GetAllEmployers(It.IsAny<string>())).ReturnsAsync(new EmprefDiscovery { Emprefs = new List<string> { ExpectedEmpref } });

        _tokenService = new Mock<ITokenServiceApiClient>();
        _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });
        _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, null, new Mock<ILogger<HmrcService>>().Object);
    }

    [Test]
    public async Task ThenTheCorrectUrlIsUsedToDiscoverTheEmpref()
    {
        //Arrange
        var authToken = "123FGV";

        //Act
        await _hmrcService.DiscoverEmpref(authToken);

        //Assert
        _apprenticeshipLevyApiClient.Verify(x => x.GetAllEmployers(authToken), Times.Once);
    }

    [Test]
    public async Task ThenTheEmprefIsReturned()
    {
        //Arrange
        var authToken = "123FGV";

        //Act
        var actual = await _hmrcService.DiscoverEmpref(authToken);

        //Assert
        Assert.AreEqual(ExpectedEmpref, actual);
    }
}