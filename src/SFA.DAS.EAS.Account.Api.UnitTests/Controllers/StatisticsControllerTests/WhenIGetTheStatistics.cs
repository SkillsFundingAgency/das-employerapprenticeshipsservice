using System.Threading.Tasks;
using FluentAssertions;
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
        private StatisticsViewModel _statisticsViewModel;

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<StatisticsOrchestrator>(null, null);

            _statisticsViewModel = new StatisticsViewModel
            {
                TotalAccounts = 1,
                TotalAgreements = 2,
                TotalLegalEntities = 3,
                TotalPayeSchemes = 4,
                TotalPayments = 5
            };

            _orchestrator.Setup(m => m.Get()).ReturnsAsync(_statisticsViewModel);

            _controller = new StatisticsController(_orchestrator.Object);
        }

        [Test]
        public async Task ThenShouldReturnOkNegotiatedContentResultWithStatistics()
        {
            //Act
            var result = await _controller.GetStatistics();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkNegotiatedContentResult<StatisticsViewModel>>();

            var okResult = (OkNegotiatedContentResult<StatisticsViewModel>)result;
            okResult.Content.Should().NotBeNull();
            okResult.Content.ShouldBeEquivalentTo(_statisticsViewModel);
        }
    }
}

