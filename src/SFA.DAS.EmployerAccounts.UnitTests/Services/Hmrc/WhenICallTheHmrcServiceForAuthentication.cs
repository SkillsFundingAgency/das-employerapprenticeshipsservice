using System.Net;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Client;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.UnitTests.Policies.Hmrc;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Hmrc;

public class WhenICallTheHmrcServiceForAuthentication
{
    private const string ExpectedAccessCode = "789654321AGFVD";
    private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/";
    private const string ExpectedClientId = "654321";
    private const string ExpectedClientSecret = "my_secret";
    private const string ExpectedOgdClientId = "123456789";
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
            OgdClientId = ExpectedOgdClientId,
            Scope = ExpectedScope,
            ClientSecret = ExpectedClientSecret
        };

        _httpClientWrapper = new Mock<IHttpClientWrapper>();
        _httpClientWrapper.Setup(x => x.SendMessage(It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse()));

        _tokenService = new Mock<ITokenServiceApiClient>();
        _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAccessCode });

        _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();

        _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object,
            _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, null, new Mock<ILogger<HmrcService>>().Object);
    }

    [Test]
    public void ThenTheAuthUrlIsGeneratedFromTheStoredConfigValues()
    {
        //Arrange
        var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";
        var urlFriendlyRedirectUrl = WebUtility.UrlEncode(redirectUrl);

        //Assert
        var actual = _hmrcService.GenerateAuthRedirectUrl(redirectUrl);

        //Assert
        Assert.AreEqual($"{ExpectedBaseUrl}oauth/authorize?response_type=code&client_id={ExpectedClientId}&scope={ExpectedScope}&redirect_uri={urlFriendlyRedirectUrl}", actual);
    }

    [Test]
    public async Task ThenTheCodeIsExchangedForTheAccessToken()
    {
        //Arrange
        var code = "ghj567";
        var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";


        //Act
        var actual = await _hmrcService.GetAuthenticationToken(redirectUrl, code);

        //Assert
        _httpClientWrapper.Verify(x => x.SendMessage(It.IsAny<object>(), "oauth/token"), Times.Once);
        Assert.IsAssignableFrom<HmrcTokenResponse>(actual);
    }
}