using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.StatisticsOrchestratorTests
{
    [TestFixture]
    [Parallelizable]
    public class WhenIGet
    {
        private StatisticsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IEmployerAccountsApiClient> _employerAccountsApiClient;
        private GetFinancialStatisticsResponse _response;
        private Statistics _statistics; 

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _employerAccountsApiClient = new Mock<IEmployerAccountsApiClient>();

            _response = new GetFinancialStatisticsResponse { TotalPayments = 5 };

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetFinancialStatisticsQuery>())).ReturnsAsync(_response);

            _statistics = new Statistics
            {
                TotalAccounts = 1,
                TotalAgreements = 2,
                TotalLegalEntities = 3,
                TotalPayeSchemes = 4
            };

            _employerAccountsApiClient.Setup(m => m.GetStatistics()).ReturnsAsync(_statistics);

            _orchestrator = new StatisticsOrchestrator(_mediator.Object, _employerAccountsApiClient.Object);
        }

        [Test]
        public async Task ThenShouldReturnStatistics()
        {
            var result = await _orchestrator.Get();

            result.Should().NotBeNull();
            result.ShouldBeEquivalentTo(new StatisticsViewModel
            {
                TotalAccounts = 1,
                TotalAgreements = 2,
                TotalLegalEntities = 3,
                TotalPayeSchemes = 4,
                TotalPayments = 5
            });
        }
    }
}
