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
        private Mock<IStatisticsOrchestrator> _orchestrator;
        private StatisticsController _controller;

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IStatisticsOrchestrator>();

            _controller = new StatisticsController(_orchestrator.Object);
        }

        [Test]
        public async Task WhenStatisticsMethodIsCalled()
        {
            SetupOrchestratorToReturnAResponse(true);
            await _controller.GetStatistics();

            _orchestrator.Verify(o => o.GetStatistics(), Times.Once);
        }

        [Test]
        public async Task ThenTheStatisticsAreReturned()
        {
            SetupOrchestratorToReturnAResponse(true);

            var actual = await _controller.GetStatistics();
            
            Assert.IsNotNull(actual);
        }

        private void SetupOrchestratorToReturnAResponse(bool withValidData)
        {
            _orchestrator.Setup(o => o.GetStatistics())
                .ReturnsAsync(new OrchestratorResponse<StatisticsViewModel>()
                {
                    Data = withValidData ? new StatisticsViewModel() : null
                });
        }

        [Test]
        public async Task ThenIfTheStatisticsAreNotAvailableReturnsNotFoundResult()
        {
            SetupOrchestratorToReturnAResponse(false);

            var actual = await _controller.GetStatistics();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}
