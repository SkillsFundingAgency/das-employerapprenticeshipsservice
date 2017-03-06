using System;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Newtonsoft.Json;
using NLog;
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
    
    class WhenICallTheHmrcServiceForDeclarations
    {
        private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private const string ExpectedClientId = "654321";
        private const string ExpectedScope = "emp_ref";
        private const string ExpectedClientSecret = "my_secret";
        private const string ExpectedAuthToken = "JGHF12345";
        private const string ExpectedOgdClientId = "123AOK564";
        private const string EmpRef = "111/ABC";

        private HmrcService _hmrcService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
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
                    Scope = ExpectedScope,
                    ClientSecret = ExpectedClientSecret,
                    OgdSecret = "ABC1234FG",
                    OgdClientId = ExpectedOgdClientId
                }
            };

            _httpClientWrapper = new Mock<IHttpClientWrapper>();

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });

            _hmrcService = new HmrcService( _configuration, _httpClientWrapper.Object, _tokenService.Object, new NoopExecutionPolicy());
        }
        
        [Test]
        public async Task ThenIShouldGetBackDeclarationsForAGivenEmpRef()
        {
            //Arrange
            var expectedApiUrl = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(EmpRef)}/declarations";
            
            var levyDeclarations = new LevyDeclarations();
            _httpClientWrapper.Setup(x => x.Get<LevyDeclarations>(It.IsAny<string>(), expectedApiUrl))
                .ReturnsAsync(levyDeclarations);

            //Act
            var result = await _hmrcService.GetLevyDeclarations(EmpRef);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<LevyDeclarations>(ExpectedAuthToken, expectedApiUrl), Times.Once);
            Assert.AreEqual(levyDeclarations, result);
        }

        [Test]
        public async Task ThenTheDateFromIsAddedToTheRequestIfPopulated()
        {
            //Arrange
            var expectedDate = new DateTime(2017,04,29);
            var expectedApiUrl = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(EmpRef)}/declarations?fromDate=2017-04-29";

            //Act
            await _hmrcService.GetLevyDeclarations(EmpRef, expectedDate);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<LevyDeclarations>(ExpectedAuthToken, expectedApiUrl), Times.Once);
        }
    }
}
