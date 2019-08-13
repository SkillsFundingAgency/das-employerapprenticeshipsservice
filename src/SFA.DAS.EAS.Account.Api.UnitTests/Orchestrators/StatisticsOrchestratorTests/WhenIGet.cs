using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.StatisticsOrchestratorTests
{
    [TestFixture]
    [Parallelizable]
    public class WhenIGet
    {
        private StatisticsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private GetFinancialStatisticsResponse _response;
        private FinancialStatisticsViewModel _financialStatistics;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();

            _financialStatistics = new FinancialStatisticsViewModel
            {
                TotalPayments = 5
            };

            _response = new GetFinancialStatisticsResponse { Statistics = _financialStatistics };

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetFinancialStatisticsQuery>())).ReturnsAsync(_response);

            _orchestrator = new StatisticsOrchestrator(_mediator.Object);
        }

        [Test]
        public async Task ThenShouldReturnFinancialStatistics()
        {
            var result = await _orchestrator.Get();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(_financialStatistics));
        }
    }
}
