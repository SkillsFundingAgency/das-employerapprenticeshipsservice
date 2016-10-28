using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcEmployer;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    public class WhenICallTheHmrcServiceForEmprefDiscovery
    {
        private const string ExpectedEmpref = "123/AB12345";
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
            _httpClientWrapper.Setup(x => x.Get<EmprefDiscovery>(It.IsAny<string>(), "apprenticeship-levy/")).ReturnsAsync(new EmprefDiscovery {Emprefs = new List<string> {ExpectedEmpref} });

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
            _httpClientWrapper.Verify(x => x.Get<EmprefDiscovery>(authToken, "apprenticeship-levy/"), Times.Once);
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
}
