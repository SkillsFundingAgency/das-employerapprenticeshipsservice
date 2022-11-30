using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Http;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Projections;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Projections;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasForecastingService
{
    public class WhenGettingAccountProjectionSummary
    {
        private const long ExpectedAccountId = 14;
        private const decimal ExpectedFundsIn = 112233.44M;
        private const decimal ExpectedFundsOut = 121212.12M;

        private EmployerFinance.Services.DasForecastingService _service;
        private Mock<IOuterApiClient> _outerApiMock;
        private Mock<ILog> _logger;

        private GetAccountProjectionSummaryResponse _expectedAccountProjectionSummaryResponse;

        private string ExpectedGetExpiringFundsUrl = $"projections/{ExpectedAccountId}";

        [SetUp]
        public void Setup()
        {
            _outerApiMock = new Mock<IOuterApiClient>();

            _expectedAccountProjectionSummaryResponse = new GetAccountProjectionSummaryResponse
            {
                AccountId = ExpectedAccountId,
                ProjectionGenerationDate = DateTime.Now,
                FundsIn = ExpectedFundsIn,
                FundsOut = ExpectedFundsOut,
                ExpiryAmounts = new List<ExpiryAmount>
                {
                    new ExpiryAmount
                    {
                        PayrollDate = DateTime.Now.AddMonths(2),
                        Amount = 200
                    }
                }
            };

            _logger = new Mock<ILog>();

            _service = new EmployerFinance.Services.DasForecastingService(_outerApiMock.Object, _logger.Object);

            _outerApiMock
                .Setup(mock => mock.Get<GetAccountProjectionSummaryResponse>(It.Is<GetAccountProjectionSummaryRequest>(x => x.GetUrl == ExpectedGetExpiringFundsUrl)))
                .ReturnsAsync(_expectedAccountProjectionSummaryResponse)
                .Verifiable();
        }

        [Test]
        public async Task ThenTheOuterApiShouldBeCalledOnTheCorrectEndpoint()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            _outerApiMock.Verify();
        }

        [Test]
        public async Task ThenIShouldReturnTheCollectedProjectionGeneratedDate()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.ProjectionGenerationDate.Should().Be(_expectedAccountProjectionSummaryResponse.ProjectionGenerationDate);
        }

        [Test]
        public async Task ThenIShouldReturnTheCollectedExpiringFunds()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.ExpiringAccountFunds.ExpiryAmounts.Count.Should().Be(1);
            result.ExpiringAccountFunds.ExpiryAmounts.First().ShouldBeEquivalentTo(_expectedAccountProjectionSummaryResponse.ExpiryAmounts.First());
        }

        [Test]
        public async Task ThenIShouldReturnTheProjectedCalculation()
        {
            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.ProjectionCalulation.FundsIn.Should().Be(ExpectedFundsIn);
            result.ProjectionCalulation.FundsOut.Should().Be(ExpectedFundsOut);
        }

        [Test]
        public async Task ThenIShouldReturnNullIfAccountCannotBeFoundOnForecastApi()
        {
            _outerApiMock.Setup(c => c.Get<GetAccountProjectionSummaryResponse>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new ResourceNotFoundException(""));

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfBadRequestSentToForecastApi()
        {
            _outerApiMock.Setup(c => c.Get<GetAccountProjectionSummaryResponse>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new HttpException(400, "Bad request"));

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }


        [Test]
        public async Task ThenIShouldReturnNullIfRequestTimesOut()
        {
            _outerApiMock.Setup(c => c.Get<GetAccountProjectionSummaryResponse>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new RequestTimeOutException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfTooManyRequests()
        {
            _outerApiMock.Setup(c => c.Get<GetAccountProjectionSummaryResponse>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new TooManyRequestsException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfForecastApiHasInternalServerError()
        {
            _outerApiMock.Setup(c => c.Get<GetAccountProjectionSummaryResponse>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new InternalServerErrorException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task ThenIShouldReturnNullIfServiceUnavailableException()
        {
            _outerApiMock.Setup(c => c.Get<GetAccountProjectionSummaryResponse>(It.IsAny<GetAccountProjectionSummaryRequest>()))
                .Throws(new ServiceUnavailableException());

            var result = await _service.GetAccountProjectionSummary(ExpectedAccountId);

            result.Should().BeNull();
        }
    }
}
