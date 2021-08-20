using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.LevyTransferMatchingServiceTests
{
    public class WhenIGetAPledgesCount
    {
        private Mock<IApiClient> _mockApiClient;
        private LevyTransferMatchingService _levyTransferMatchingService;

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IApiClient>();

            _levyTransferMatchingService = new LevyTransferMatchingService(_mockApiClient.Object);
        }

        [Test]
        public async Task ThenTheOuterApiIsCalledAndTotalPledgesForAccountReturned()
        {
            var expectedResult = 567;

            var accountId = 123;

            _mockApiClient
                .Setup(x => x.Get<GetPledgesResponse>(It.Is<GetPledgesRequest>(y => y.GetUrl.EndsWith(accountId.ToString()))))
                .ReturnsAsync(new GetPledgesResponse()
                {
                    TotalPledges = expectedResult,
                });

            var actualResult = await _levyTransferMatchingService.GetPledgesCount(accountId);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}