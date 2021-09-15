using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.LevyTransferMatchingServiceTests
{
    public class WhenIGetAPledgesCount
    {
        private Mock<IApiClient> _mockApiClient;
        private ManageApprenticeshipsService _levyTransferMatchingService;

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IApiClient>();

            _levyTransferMatchingService = new ManageApprenticeshipsService(_mockApiClient.Object);
        }

        [Test]
        public async Task ThenTheOuterApiIsCalledAndTotalPledgesForAccountReturned()
        {
            var expectedResult = 567;

            var accountId = 123;

            _mockApiClient
                .Setup(x => x.Get<GetIndexResponse>(It.Is<GetIndexRequest>(y => y.GetUrl.EndsWith(accountId.ToString()))))
                .ReturnsAsync(new GetIndexResponse()
                {
                    PledgesCount = expectedResult,
                });

            var actualResult = await _levyTransferMatchingService.GetIndex(accountId);

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}