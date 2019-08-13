using System.Threading.Tasks;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.StatisticsControllerTests
{
    [TestFixture]
    public class WhenICallTheStatisticsEndPoint
    {
        private StatisticsController _controller;
        private Mock<StatisticsOrchestrator> _orchestrator;
        private FinancialStatisticsViewModel _financialStatistics;

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<StatisticsOrchestrator>(null);

            _financialStatistics = new FinancialStatisticsViewModel
            {
                TotalPayments = 5
            };

            _orchestrator.Setup(m => m.Get()).ReturnsAsync(_financialStatistics);

            _controller = new StatisticsController(_orchestrator.Object);
        }

        [Test]
        public async Task ThenShouldReturnFinancialStatistics()
        {
            var result = await _controller.GetStatistics() as OkNegotiatedContentResult<FinancialStatisticsViewModel>; ;
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(_financialStatistics));
        }
    }
}

