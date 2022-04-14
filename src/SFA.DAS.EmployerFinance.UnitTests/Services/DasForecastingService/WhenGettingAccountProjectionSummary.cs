using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Http;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasForecastingService
{
    public class WhenGettingAccountProjectionSummary
    {
        private const long ExpectedAccountId = 14;
        private const decimal ExpectedFundsIn = 112233.44M;
        private const decimal ExpectedFundsOut = 121212.12M;
        private const string AccessToken = "sercure_token";

        private EmployerFinance.Services.DasForecastingService _service;
        private ForecastingApiClientConfiguration _apiClientConfiguration;
        private Mock<IApiClient> _outerApiMock;
        private Mock<IHttpClientWrapper> _httpClient;
        private Mock<IAzureAdAuthenticationService> _azureAdAuthService;
        private Mock<ILog> _logger;

        private ExpiringAccountFunds _expectedAccountExpiringFunds;
        private AccountProjectionSummaryResponseItem _expectedAccountExpiringFundsResponse;

        private string ExpectedGetExpiringFundsUrl = $"account/{ExpectedAccountId}/expiring-funds";

        [SetUp]
        public void Setup()
        {
            _outerApiMock = new Mock<IApiClient>();
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
                ExpiryAmounts = new List<ExpiringFunds>
                {
                    new ExpiringFunds
                    {
                        PayrollDate = DateTime.Now.AddMonths(2),
                        Amount = 200
                    }
                }
            };

            _expectedAccountExpiringFundsResponse = new AccountProjectionSummaryResponseItem
            {
                AccountId = ExpectedAccountId,
                ProjectionGenerationDate = DateTime.Now,
                FundsIn = ExpectedFundsIn,
                FundsOut = ExpectedFundsOut,
                ExpiryAmounts = new List<ExpiringFundsReponseItem>
                {
                    new ExpiringFundsReponseItem
                    {
                        PayrollDate = DateTime.Now.AddMonths(2),
                        Amount = 200
                    }
                }
            };

            _logger = new Mock<ILog>();

            _service = new EmployerFinance.Services.DasForecastingService(_httpClient.Object, _azureAdAuthService.Object, _apiClientConfiguration, _outerApiMock.Object, _logger.Object);

            _httpClient.Setup(c => c.Get<ExpiringAccountFunds>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_expectedAccountExpiringFunds);

            _outerApiMock
                .Setup(mock => mock.Get<AccountProjectionSummaryResponseItem>(It.Is<GetAccountProjectionSummaryRequest>(x => x.GetUrl == ExpectedGetExpiringFundsUrl)))
                .Callback((IGetApiRequest r) =>
                {
                    var a = r;
                })
                .ReturnsAsync(_expectedAccountExpiringFundsResponse)
                .Verifiable();

            _azureAdAuthService.Setup(s => s.GetAuthenticationResult(
                _apiClientConfiguration.ClientId,
                _apiClientConfiguration.ClientSecret,
                _apiClientConfiguration.IdentifierUri,
                _apiClientConfiguration.Tenant)).ReturnsAsync(AccessToken);
        }

        [Test]
        public async Task ThenTheOuterApiShouldBeCalledOnTheCorrectEndpoint()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            _outerApiMock.Verify();
        }

        [Test]
        public async Task ThenIShouldReturnTheCollectedExpiryDate()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.ProjectionGenerationDate.Should().Be(_expectedAccountExpiringFundsResponse.ProjectionGenerationDate);
        }

        [Test]
        public async Task ThenIShouldReturnTheCollectedExpiringFunds()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.ExpiringAccountFunds.ExpiryAmounts.Count.Should().Be(1);
            result.ExpiringAccountFunds.ExpiryAmounts.First().ShouldBeEquivalentTo(_expectedAccountExpiringFundsResponse.ExpiryAmounts.First());
        }

        [Test]
        public async Task ThenIShouldReturnNullIfAccountCannotBeFoundOnForecastApi()
        {
            _outerApiMock.Setup(c => c.Get<AccountProjectionSummaryResponseItem>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new ResourceNotFoundException(""));

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfBadRequestSentToForecastApi()
        {
            _outerApiMock.Setup(c => c.Get<AccountProjectionSummaryResponseItem>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new HttpException(400, "Bad request"));

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }


        [Test]
        public async Task ThenIShouldReturnNullIfRequestTimesOut()
        {
            _outerApiMock.Setup(c => c.Get<AccountProjectionSummaryResponseItem>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new RequestTimeOutException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfTooManyRequests()
        {
            _outerApiMock.Setup(c => c.Get<AccountProjectionSummaryResponseItem>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new TooManyRequestsException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfForecastApiHasInternalServerError()
        {
            _outerApiMock.Setup(c => c.Get<AccountProjectionSummaryResponseItem>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new InternalServerErrorException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfServiceUnavailableException()
        {
            _outerApiMock.Setup(c => c.Get<AccountProjectionSummaryResponseItem>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new ServiceUnavailableException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }
    }
}
