using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.StatisticsControllerTests
{
    [TestFixture]
    public class WhenICallTheStatisticsEndPoint
    {
        private StatisticsController _controller;
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

            _controller = new StatisticsController(_mediator.Object);
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

