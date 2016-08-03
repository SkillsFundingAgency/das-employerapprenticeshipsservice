using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    public class WhenICallTheHmrcServiceForEmprefDiscovery
    {
        private HmrcService _hmrcService;
        private Mock<ILogger> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private string ExpectedClientId = "654321";
        private string ExpectedScope = "emp_ref";
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private string ExpectedClientSecret = "my_secret";
        private string ExpectedName = "My Company Name";

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
            _httpClientWrapper.Setup(x => x.Get<string>(It.IsAny<string>(), "apprenticeship-levy")).ReturnsAsync("{\"_links\": {\"self\": {\"href\": \"/\"},\"123/AB12345\": {\"href\": \"/epaye/123%2FAB12345\"}}}");

            _hmrcService = new HmrcService(_logger.Object, _configuration, _httpClientWrapper.Object);
        }

        [Test]
        public async Task ThenTheCorrectUrlIsUsedToDiscoverTheEmpref()
        {
            //Arrange
            var authToken = "123FGV";

            //Act
            await _hmrcService.DiscoverEmpref(authToken);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<string>(authToken, "apprenticeship-levy"), Times.Once);
        }

        [Test]
        public async Task ThenTheEmprefIsReturned()
        {
            //Arrange
            var authToken = "123FGV";

            //Act
            var actual = await _hmrcService.DiscoverEmpref(authToken);

            //Assert
            Assert.AreEqual("123/AB12345", actual);
        }
    }
}
