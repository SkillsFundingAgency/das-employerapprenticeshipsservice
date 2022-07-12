using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.LevyTransferMatchingServiceTests
{
    public class WhenIGetCounts
    {
        private Mock<IOuterApiClient> _mockApiClient;
        private TransfersService _transfersService;

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IOuterApiClient>();

            _transfersService = new TransfersService(_mockApiClient.Object);
        }

        [Test]
        public async Task ThenTheOuterApiIsCalledAndTotalPledgesForAccountReturned()
        {
            var expectedResult = 567;

            var accountId = 123;

            _mockApiClient
                .Setup(x => x.Get<GetCountsResponse>(It.Is<GetCountsRequest>(y => y.GetUrl.EndsWith($"{accountId}/counts"))))
                .ReturnsAsync(new GetCountsResponse()
                {
                    PledgesCount = expectedResult,
                });

            var actualResult = await _transfersService.GetCounts(accountId);

            Assert.AreEqual(expectedResult, actualResult.PledgesCount);
        }
    }
}