using System.Threading.Tasks;
using System.Web;
using Moq;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    public class WhenICallTheHmrcServiceForAuthentication
    {
        private HmrcService _hmrcService;
        private Mock<ILogger> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private string ExpectedBaseUrl = "http://hmrcbase.gov.uk/";
        private string ExpectedClientId = "654321";
        private string ExpectedOgdClientId = "123456789";
        private string ExpectedScope = "emp_ref";
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private string ExpectedClientSecret = "my_secret";
        private const string ExpectedTotpToken = "789654321AGFVD";
        private Mock<ITotpService> _totpService;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration
                {
                    BaseUrl = ExpectedBaseUrl,
                    ClientId = ExpectedClientId,
                    OgdClientId = ExpectedOgdClientId,
                    Scope = ExpectedScope,
                    ClientSecret = ExpectedClientSecret
                }
            };

            _logger = new Mock<ILogger>();
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse()));

            _totpService = new Mock<ITotpService>();
            _totpService.Setup(x => x.GetCode(It.IsAny<string>())).Returns(ExpectedTotpToken);

            _hmrcService = new HmrcService(_logger.Object, _configuration, _httpClientWrapper.Object, _totpService.Object);
        }

        [Test]
        public void ThenTheAuthUrlIsGeneratedFromTheStoredConfigValues()
        {
            //Arrange
            var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);

            //Assert
            var actual = _hmrcService.GenerateAuthRedirectUrl(redirectUrl);

            //Assert
            Assert.AreEqual($"{ExpectedBaseUrl}oauth/authorize?response_type=code&client_id={ExpectedClientId}&scope={ExpectedScope}&redirect_uri={urlFriendlyRedirectUrl}", actual);
        }

        [Test]
        public async Task ThenTheCodeIsExchangedForTheAccessToken()
        {
            //Arrange
            var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";
            var urlFriendlyRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
            var code = "ghj567";

            //Act
            var actual = await _hmrcService.GetAuthenticationToken(redirectUrl,code);

            //Assert
            _httpClientWrapper.Verify(x => x.SendMessage("", $"oauth/token?client_secret={ExpectedClientSecret}&client_id={ExpectedClientId}&grant_type=authorization_code&redirect_uri={urlFriendlyRedirectUrl}&code={code}"), Times.Once);
            Assert.IsAssignableFrom<HmrcTokenResponse>(actual);

        }

        [Test]
        public async Task ThenTheOgdSecretIsUsedToGenerateAnAccessToken()
        {
            //Act
            var actual = await _hmrcService.GetOgdAuthenticationToken();

            //Assert
            _totpService.Verify(x=>x.GetCode(It.IsAny<string>()),Times.Once);
            _httpClientWrapper.Verify(x => x.SendMessage("", $"oauth/token?client_secret={ExpectedTotpToken}&client_id={ExpectedOgdClientId}&grant_type=client_credentials&scopes=read:apprenticeship-levy"), Times.Once);
            Assert.IsAssignableFrom<HmrcTokenResponse>(actual);
        }
    }
}
