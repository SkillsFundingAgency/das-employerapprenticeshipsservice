using System;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    class WhenICallHmrcServiceForLastEnglishFractionUpdateDate
    {
        private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private const string ExpectedClientId = "654321";
        private const string ExpectedScope = "emp_ref";
        private const string ExpectedClientSecret = "my_secret";
        private const string ExpectedAccessCode = "789654321AGFVD";

        private HmrcService _hmrcService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        
        private Mock<ITokenServiceApiClient> _tokenService;
        private Mock<ICacheProvider> _cacheProvider;

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
                    ServerToken = "token1234"
                }
            };
            
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            
            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken {AccessCode = ExpectedAccessCode});

            _cacheProvider = new Mock<ICacheProvider>();
            _cacheProvider.SetupSequence(c => c.Get<DateTime?>("HmrcFractionLastCalculatedDate"))
                .Returns(null)
                .Returns(new DateTime());

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _tokenService.Object, new NoopExecutionPolicy(),_cacheProvider.Object);
        }

        [Test]
        public async Task ThenIShouldGetTheCurrentUpdatedDate()
        {
            //Assign
            const string expectedApiUrl = "apprenticeship-levy/fraction-calculation-date";
            var updateDate = DateTime.Now;
            
            _httpClientWrapper.Setup(x => x.Get<DateTime>(It.IsAny<string>(), expectedApiUrl))
                .ReturnsAsync(updateDate);

            //Act
            var result = await _hmrcService.GetLastEnglishFractionUpdate();

            //Assert
            _httpClientWrapper.Verify(x => x.Get<DateTime>(ExpectedAccessCode, expectedApiUrl), Times.Once);
            Assert.AreEqual(updateDate, result);
        }


        [Test]
        public async Task ThenTheFractionLastCaclulatedDateIsReadFromTheCacheOnSubsequentReads()
        {
            //Arrange
            const string expectedApiUrl = "apprenticeship-levy/fraction-calculation-date";
            
            //Act
            await _hmrcService.GetLastEnglishFractionUpdate();
            await _hmrcService.GetLastEnglishFractionUpdate();

            //Assert
            _httpClientWrapper.Verify(x => x.Get<DateTime>(ExpectedAccessCode, expectedApiUrl), Times.Once);
            _cacheProvider.Verify(x => x.Set("HmrcFractionLastCalculatedDate", It.IsAny<DateTime>(), It.Is<TimeSpan>(c => c.Days.Equals(1))));
        }
    }
}
