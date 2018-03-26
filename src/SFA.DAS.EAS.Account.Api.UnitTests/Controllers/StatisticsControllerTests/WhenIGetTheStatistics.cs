using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task TheOrchestratorGetTheRdsRequiredStatisticsMethodIsCalled()
        {
            SetupOrchestratorToReturnAResponse(true);
            await _controller.GetTheRdsStatistics();

            _orchestrator.Verify(o => o.GetTheRdsRequiredStatistics(), Times.Once);
        }

        [Test]
        public async Task ThenTheStatisticsAreReturned()
        {
            SetupOrchestratorToReturnAResponse(true);

            var actual = await _controller.GetTheRdsStatistics();
            
            Assert.IsNotNull(actual);
        }

        private void SetupOrchestratorToReturnAResponse(bool withValidData)
        {
            _orchestrator.Setup(o => o.GetTheRdsRequiredStatistics())
                .ReturnsAsync(new OrchestratorResponse<RdsRequiredStatisticsViewModel>()
                {
                    Data = withValidData ? new RdsRequiredStatisticsViewModel() : null
                });
        }

        [Test]
        public async Task ThenIfTheStatisticsAreNotAvailableReturnsNotFoundResult()
        {
            SetupOrchestratorToReturnAResponse(false);

            var actual = await _controller.GetTheRdsStatistics();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}
