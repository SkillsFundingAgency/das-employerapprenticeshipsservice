using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasForecastingService
{
    public class WhenGettingProjectedCalculations
    {
        private const long ExpectedAccountId = 14;
        private const decimal ExpectedFundsIn = 112233.44M;
        private const decimal ExpectedFundsOut = 121212.12M;
        private const string AccessToken = "sercure_token";

        private EmployerFinance.Services.DasForecastingService _service;
        private ForecastingApiClientConfiguration _apiClientConfiguration;
        private Mock<IHttpClientWrapper> _httpClient;
        private Mock<IAzureAdAuthenticationService> _azureAdAuthService;
        private Mock<ILog> _logger;

        private ProjectedCalculation _expectedProjectedCalculations;

        [SetUp]
        public void Setup()
        {
            _httpClient = new Mock<IHttpClientWrapper>();
            _azureAdAuthService = new Mock<IAzureAdAuthenticationService>();

            _apiClientConfiguration = new ForecastingApiClientConfiguration
            {
                ApiBaseUrl = "testUrl",
                ClientId = "clientId",
                ClientSecret = "secret",
                IdentifierUri = "test",
                Tenant = "tenant"
            };

            _expectedProjectedCalculations = new ProjectedCalculation
            {
                AccountId = ExpectedAccountId,
                ProjectionGenerationDate = DateTime.Now,
                FundsIn = ExpectedFundsIn,
                FundsOut = ExpectedFundsOut,
                NumberOfMonths = 12
            };
            
            _logger = new Mock<ILog>();

            _service = new EmployerFinance.Services.DasForecastingService(_httpClient.Object, _azureAdAuthService.Object, _apiClientConfiguration, _logger.Object);

            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_expectedProjectedCalculations);

            _azureAdAuthService.Setup(s => s.GetAuthenticationResult(
                _apiClientConfiguration.ClientId,
                _apiClientConfiguration.ClientSecret,
                _apiClientConfiguration.IdentifierUri,
                _apiClientConfiguration.Tenant)).ReturnsAsync(AccessToken);
        }

        [Test]
        public async Task ThenTheForecastApiShouldBeCalledWithTheCorrectCredentials()
        {
            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            _httpClient.Verify(c => c.Get<ProjectedCalculation>(
                AccessToken,
                $"/api/accounts/{ExpectedAccountId}/AccountProjection/projected-summary"), Times.Once);
        }

        [Test]
        public async Task ThenIShouldReturnTheProjectionGenerationDate()
        {
            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.ProjectionGenerationDate.Should().Be(_expectedProjectedCalculations.ProjectionGenerationDate);
        }

        [Test]
        public async Task ThenIShouldReturnTheProjectedCalculation()
        {
            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.FundsIn.Should().Be(ExpectedFundsIn);
            result.FundsOut.Should().Be(ExpectedFundsOut);
        }

        [Test]
        public async Task ThenIShouldReturnNullIfAccountCannotBeFoundOnForecastApi()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ResourceNotFoundException(""));

            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfBadRequestSentToForecastApi()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new HttpException(400, "Bad request"));

            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.Should().BeNull();
        }

       
        [Test]
        public async Task ThenIShouldReturnNullIfRequestTimesOut()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new RequestTimeOutException());

            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfTooManyRequests()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new TooManyRequestsException());

            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfForecastApiHasInternalServerError()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InternalServerErrorException());

            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfServiceUnavailableException()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new RequestTimeOutException());

            var result = await _service.GetProjectedCalculations(ExpectedAccountId);

            result.Should().BeNull();
        }
        
        [Test]
        public void ThenIShouldThrowExceptionIfUnexpectedHttpStatusCode()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new HttpException(501, "Service not implemented"));

            Assert.ThrowsAsync<HttpException>(() => _service.GetProjectedCalculations(ExpectedAccountId));
        }

        [Test]
        public void ThenIShouldThrowExceptionIfUnexpected()
        {
            _httpClient.Setup(c => c.Get<ProjectedCalculation>(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            Assert.ThrowsAsync<Exception>(() => _service.GetProjectedCalculations(ExpectedAccountId));
        }
    }
}
