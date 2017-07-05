using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Queries.GetPriceHistoryQueryRequest;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetApprenticeshipPriceHistory
{
    [TestFixture]
    public class WhenGettingApprenticeshipPriceHistory
    {
        private Mock<IEmployerCommitmentApi> _commitmentApi;
        private GetPriceHistoryQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _commitmentApi = new Mock<IEmployerCommitmentApi>();
            _commitmentApi.Setup(x => x.GetPriceHistory(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new List<PriceHistory>());

            _handler = new GetPriceHistoryQueryHandler(_commitmentApi.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToRetrieveData()
        {
            //Arrange
            var query = new GetPriceHistoryQueryRequest
            {
                ApprenticeshipId = 1
            };

            //Act
            await _handler.Handle(query);

            //Assert
            _commitmentApi.Verify(x => x.GetPriceHistory(It.IsAny<long>(), 1), Times.Once);
        }
    }
}
