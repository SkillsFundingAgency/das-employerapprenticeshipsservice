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
    class WhenICallTheHmrcServiceForEnglishFractions
    {
        private const string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private const string ExpectedClientId = "654321";
        private const string ExpectedScope = "emp_ref";
        private const string ExpectedClientSecret = "my_secret";
        private const string ExpectedTotpToken = "789654321AGFVD";
        private const string ExpectedAuthToken = "GFRT567";

        private HmrcService _hmrcService;
        private Mock<ILogger> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
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
                    Scope = ExpectedScope,
                    ClientSecret = ExpectedClientSecret
                }
            };

            _logger = new Mock<ILogger>();
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.SendMessage("", $"oauth/token?client_secret={ExpectedTotpToken}&client_id={ExpectedClientId}&grant_type=client_credentials&scopes=read:apprenticeship-levy")).ReturnsAsync(JsonConvert.SerializeObject(new HmrcTokenResponse { AccessToken = ExpectedAuthToken }));

            _totpService = new Mock<ITotpService>();
            _totpService.Setup(x => x.GetCode(It.IsAny<string>())).Returns(ExpectedTotpToken);

            _hmrcService = new HmrcService(_logger.Object, _configuration, _httpClientWrapper.Object,_totpService.Object);
        }

        [Test]
        public async Task ThenIShouldGetBackDeclarationsForAGivenEmpRef()
        {
            //Assign
            const string empRef = "111/ABC";
            var expectedApiUrl = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/fractions";

            var englishFractions = new EnglishFractionDeclarations();
            _httpClientWrapper.Setup(x => x.Get<EnglishFractionDeclarations>(It.IsAny<string>(), expectedApiUrl))
                .ReturnsAsync(englishFractions);

            //Act
            var result = await _hmrcService.GetEnglishFractions(empRef);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<EnglishFractionDeclarations>(ExpectedAuthToken, expectedApiUrl), Times.Once);
            Assert.AreEqual(englishFractions, result);
        }
    }
}
