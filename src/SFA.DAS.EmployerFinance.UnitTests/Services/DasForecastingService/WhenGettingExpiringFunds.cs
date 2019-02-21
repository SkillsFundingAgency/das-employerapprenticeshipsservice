using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasForecastingService
{
    public class WhenGettingExpiringFunds
    {
        private const long ExpectedAccountId = 14;
        private const string AccessToken = "sercure_token";

        private EmployerFinance.Services.DasForecastingService _service;
        private ForecastingApiClientConfiguration _apiClientConfiguration;
        private Mock<IHttpClientWrapper> _httpClient;
        private Mock<IAzureAdAuthenticationService> _azureAdAuthService;
        private Mock<ILog> _logger;

        private ExpiringAccountFunds _expectedAccountExpiringFunds;


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

            _expectedAccountExpiringFunds = new ExpiringAccountFunds
            {
                AccountId = ExpectedAccountId,
                ProjectionGenerationDate = DateTime.Now,
                ExpiryAmounts = new List<ExpiringFunds>
                {
                    new ExpiringFunds
                    {
                        PayrollDate = DateTime.Now.AddMonths(2),
                        Amount = 200
                    }
                }
            };
            
            _logger = new Mock<ILog>();

            _service = new EmployerFinance.Services.DasForecastingService(_httpClient.Object, _azureAdAuthService.Object, _apiClientConfiguration, _logger.Object);

            _httpClient.Setup(c => c.Get<ExpiringAccountFunds>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_expectedAccountExpiringFunds);

            _azureAdAuthService.Setup(s => s.GetAuthenticationResult(
                _apiClientConfiguration.ClientId,
                _apiClientConfiguration.ClientSecret,
                _apiClientConfiguration.IdentifierUri,
                _apiClientConfiguration.Tenant)).ReturnsAsync(AccessToken);
        }

        [Test]
        public async Task ThenTheForecastApiShouldBeCalledWithTheCorrectCredentials()
        {
            var result = await _service.GetExpiringAccountFunds(ExpectedAccountId);

            _httpClient.Verify(c => c.Get<ExpiringAccountFunds>(
                AccessToken,
                $"/api/accounts/{ExpectedAccountId}/AccountProjection/expiring-funds"), Times.Once);
        }

        [Test]
        public async Task ThenIShouldReturnTheCollectedExpiryDate()
        {
            var result = await _service.GetExpiringAccountFunds(ExpectedAccountId);

            result.ProjectionGenerationDate.Should().Be(_expectedAccountExpiringFunds.ProjectionGenerationDate);
        }

        [Test]
        public async Task ThenIShouldReturnTheCollectedExpiringFunds()
        {
            var result = await _service.GetExpiringAccountFunds(ExpectedAccountId);

            result.ExpiryAmounts.Count.Should().Be(1);
            result.ExpiryAmounts.First().Should().Be(_expectedAccountExpiringFunds.ExpiryAmounts.First());
        }
    }
}
