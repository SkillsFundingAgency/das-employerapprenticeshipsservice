using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetTheRdsRequiredStatistics;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.StatisticsOrchestratorTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        private StatisticsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();

            _orchestrator = new StatisticsOrchestrator(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsInvoked()
        {
            SetupTheMediatorToReturnAResponse(true);

            await _orchestrator.GetTheRdsRequiredStatistics();

            _mediator.Verify(o => o.SendAsync(It.IsAny<GetTheRdsRequiredStatisticsRequest>()), Times.Once);
        }

        [Test]
        public async Task ThenWhenTheMediatorResponseIsReturnedItReturnsTheOrchestratorResponse()
        {
            SetupTheMediatorToReturnAResponse(true);

            var actual = await _orchestrator.GetTheRdsRequiredStatistics();
            
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenWhenTheMediatorResponseIsReturnedAndItIsAnEmptyObjectItReturnsNullInTheOrchestratorResponse()
        {
            SetupTheMediatorToReturnAResponse(false);

            var actual = await _orchestrator.GetTheRdsRequiredStatistics();

            Assert.IsNotNull(actual);
            Assert.IsNull(actual.Data);
        }

        private void SetupTheMediatorToReturnAResponse(bool returnPopulatedModel)
        {
            _mediator.Setup(o => o.SendAsync(It.IsAny<GetTheRdsRequiredStatisticsRequest>()))
                .ReturnsAsync(new GetTheRdsRequiredStatisticsResponse
                {
                    Statistics = returnPopulatedModel ? new RdsRequiredStatisticsViewModel()
                    {
                        TotalPayments = 1
                    } : new RdsRequiredStatisticsViewModel()
                });
        }
    }
}
