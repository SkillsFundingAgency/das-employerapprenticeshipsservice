using System.Threading.Tasks;
using System.Web;
using Moq;
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
    public class WhenICallTheHmrcServiceForEmpref
    {
        private string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private string ExpectedClientId = "654321";
        private string ExpectedScope = "emp_ref";
        private string ExpectedClientSecret = "my_secret";
        private string ExpectedName = "My Company Name";
        private const string ExpectedAuthToken = "789654321AGFVD";

        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private HmrcService _hmrcService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
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
                    ClientSecret = ExpectedClientSecret
                }
            };
            
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.Get<EmpRefLevyInformation>(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EmpRefLevyInformation { Employer = new Employer {Name= new Name {EmprefAssociatedName = ExpectedName} }, Links = new Links() });

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });

            _hmrcService = new HmrcService( _configuration, _httpClientWrapper.Object, _tokenService.Object, new NoopExecutionPolicy(),null);
        }

        [Test]
        public async Task ThenTheCorrectUrlIsUsedToGetTheEmprefInformation()
        {
            //Arrange
            var authToken = "123FGV";
            var empRef = "123/AB12345";

            //Act
            await _hmrcService.GetEmprefInformation(authToken, empRef);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<EmpRefLevyInformation>(authToken, $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}"), Times.Once);
            
        }

        [Test]
        public async Task ThenTheLevInformationIsReturned()
        {
            //Arrange
            var authToken = "123FGV";
            var empRef = "123/AB12345";

            //Act
            var actual = await _hmrcService.GetEmprefInformation(authToken, empRef);
            Assert.IsAssignableFrom<EmpRefLevyInformation>(actual);
            Assert.AreEqual(ExpectedName,actual.Employer.Name.EmprefAssociatedName);
        }
    }
}