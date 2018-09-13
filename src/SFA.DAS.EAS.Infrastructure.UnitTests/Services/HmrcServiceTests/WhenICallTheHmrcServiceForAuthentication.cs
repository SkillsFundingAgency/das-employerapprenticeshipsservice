﻿using System.Threading.Tasks;
using System.Web;
using HMRC.ESFA.Levy.Api.Client;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;
using System.Threading.Tasks;
using System.Web;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    public class WhenICallTheHmrcServiceForAuthentication
    {
        private HmrcService _hmrcService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private string ExpectedBaseUrl = "http://hmrcbase.gov.uk/";
        private string ExpectedClientId = "654321";
        private string ExpectedOgdClientId = "123456789";
        private string ExpectedScope = "emp_ref";
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private Mock<IApprenticeshipLevyApiClient> _apprenticeshipLevyApiClient;
        private string ExpectedClientSecret = "my_secret";
        private const string ExpectedAccessCode = "789654321AGFVD";
        private Mock<ITokenServiceApiClient> _tokenService;

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

            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.SendMessage(It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse()));

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAccessCode });

            _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object,
                _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(), null, null, new Mock<ILog>().Object);
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
            var code = "ghj567";
            var redirectUrl = "http://mytestUrl.to.redirectto?a=564kjg";


            //Act
            var actual = await _hmrcService.GetAuthenticationToken(redirectUrl, code);

            //Assert
            _httpClientWrapper.Verify(x => x.SendMessage(It.IsAny<object>(), "oauth/token"), Times.Once);
            Assert.IsAssignableFrom<HmrcTokenResponse>(actual);

        }

    }
}
