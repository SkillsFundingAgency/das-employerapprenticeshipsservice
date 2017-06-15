using System;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Domain.Models.Employer;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.CompaniesHouseEmployerVerificationServiceTests
{
    public class WhenIGetCompanyInformation
    {
        private CompaniesHouseEmployerVerificationService _companiesHouseEmployerVerificationService;
        private Mock<ILog> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private const string ExpectedCompanyId = "123EDC";
        private const string ExpectedBaseUrl = "http://some-url/";
        private const string ExpectedApiKey = "12345TGFSD2";

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILog>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                CompaniesHouse = new CompaniesHouseConfiguration
                {
                    BaseUrl = ExpectedBaseUrl,
                    ApiKey = ExpectedApiKey
                }
            };

            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(
                x => x.Get<EmployerInformation>(Convert.ToBase64String(Encoding.UTF8.GetBytes(_configuration.CompaniesHouse.ApiKey)),
                                                $"{_configuration.CompaniesHouse.BaseUrl}/company/{ExpectedCompanyId}"))
                              .ReturnsAsync(new EmployerInformation());



            _companiesHouseEmployerVerificationService = new CompaniesHouseEmployerVerificationService(_configuration, _logger.Object, _httpClientWrapper.Object, new NoopExecutionPolicy());
        }

        [Test]
        public async Task ThenTheCorrectUrlIsUsedFromConfiguration()
        {
            //Act
            var actual = await _companiesHouseEmployerVerificationService.GetInformation(ExpectedCompanyId);

            //Assert
            Assert.IsNotNull(actual);
            _httpClientWrapper.Verify(x => x.Get<EmployerInformation>(Convert.ToBase64String(Encoding.UTF8.GetBytes(_configuration.CompaniesHouse.ApiKey)), $"{_configuration.CompaniesHouse.BaseUrl}/company/{ExpectedCompanyId}"), Times.Once);
        }


        [Test]
        public async Task ThenTheNullIsReturnedWhenThereAreNoResultsBack()
        {
            //Act
            var actual = await _companiesHouseEmployerVerificationService.GetInformation("TEST");

            //Assert
            Assert.IsNull(actual);
        }
    }
}
