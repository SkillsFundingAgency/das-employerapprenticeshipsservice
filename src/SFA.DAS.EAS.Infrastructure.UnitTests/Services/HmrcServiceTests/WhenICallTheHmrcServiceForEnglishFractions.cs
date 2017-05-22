using System;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    class WhenICallTheHmrcServiceForEnglishFractions
    {
        private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private const string ExpectedClientId = "654321";
        private const string ExpectedScope = "emp_ref";
        private const string ExpectedClientSecret = "my_secret";
        private const string ExpectedTotpToken = "789654321AGFVD";
        private const string ExpectedAuthToken = "GFRT567";
        private const string ExpectedOgdClientId = "123AOK564";
        private const string EmpRef = "111/ABC";

        private HmrcService _hmrcService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private Mock<ITokenServiceApiClient> _tokenService;
        private Mock<IAzureAdAuthenticationService> _azureAdAuthService;


        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration
                {
                    BaseUrl = ExpectedBaseUrl,
                    ClientId = ExpectedClientId,
                    Scope = ExpectedScope,
                    ClientSecret = ExpectedClientSecret,
                    OgdClientId = ExpectedOgdClientId,
                    AzureAppKey = "123TRG",
                    AzureClientId = "TYG567",
                    AzureResourceId = "Resource1",
                    AzureTenant = "test",
                    UseHiDataFeed = false
                }
            };
            
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.SendMessage("", $"oauth/token?client_secret={ExpectedTotpToken}&client_id={ExpectedOgdClientId}&grant_type=client_credentials&scopes=read:apprenticeship-levy")).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse { AccessToken = ExpectedAuthToken }));

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });

            _azureAdAuthService = new Mock<IAzureAdAuthenticationService>();
            _azureAdAuthService.Setup(x =>
                    x.GetAuthenticationResult(_configuration.Hmrc.AzureClientId, _configuration.Hmrc.AzureAppKey,
                        _configuration.Hmrc.AzureResourceId, _configuration.Hmrc.AzureTenant))
                .ReturnsAsync(ExpectedAuthToken);

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _tokenService.Object, new NoopExecutionPolicy(),null, _azureAdAuthService.Object);
        }

        [Test]
        public async Task ThenIShouldGetBackDeclarationsForAGivenEmpRef()
        {
            //Arrange
            
            var expectedApiUrl = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(EmpRef)}/fractions";

            var englishFractions = new EnglishFractionDeclarations();
            _httpClientWrapper.Setup(x => x.Get<EnglishFractionDeclarations>(It.IsAny<string>(), expectedApiUrl))
                .ReturnsAsync(englishFractions);

            //Act
            var result = await _hmrcService.GetEnglishFractions(EmpRef);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<EnglishFractionDeclarations>(ExpectedAuthToken, expectedApiUrl), Times.Once);
            Assert.AreEqual(englishFractions, result);
        }

        [Test]
        public async Task ThenTheFractionsAreFilterByTheDateRangeWhenPassed()
        {
            //Arrange
            var expectedDate = new DateTime(2017, 04, 29);
            var expectedApiUrl = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(EmpRef)}/fractions?fromDate=2017-04-29";

            //Act
            await _hmrcService.GetEnglishFractions(EmpRef, expectedDate);
            _httpClientWrapper.Verify(x => x.Get<EnglishFractionDeclarations>(ExpectedAuthToken, expectedApiUrl), Times.Once);
        }


        [Test]
        public async Task ThenIfTheConfigurationIsSetToUseTheMiDataThenTheAzureAuthServiceIsCalled()
        {
            //Arrange
            _configuration.Hmrc.UseHiDataFeed = true;


            //Act
            await _hmrcService.GetLevyDeclarations(EmpRef);

            //Assert
            _tokenService.Verify(x => x.GetPrivilegedAccessTokenAsync(), Times.Never);
            _azureAdAuthService.Verify(x => x.GetAuthenticationResult(_configuration.Hmrc.ClientId, _configuration.Hmrc.AzureAppKey, _configuration.Hmrc.AzureResourceId, _configuration.Hmrc.AzureTenant), Times.Once);
        }
    }
}
