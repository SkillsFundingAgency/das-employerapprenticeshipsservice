using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;
using SFA.DAS.Http;

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
        private Mock<IApprenticeshipLevyApiClient> _apprenticeshipLevyApiClient;
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

            _apprenticeshipLevyApiClient = new Mock<IApprenticeshipLevyApiClient>();
            _apprenticeshipLevyApiClient.Setup(x => x.GetEmployerDetails(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new EmpRefLevyInformation
                {
                    Employer = new Employer {Name = new Name {EmprefAssociatedName = ExpectedName}},
                    Links = new Links()
                });

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });

            _hmrcService = new HmrcService( _configuration, _httpClientWrapper.Object, _apprenticeshipLevyApiClient.Object, _tokenService.Object, new NoopExecutionPolicy(),null,null);
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