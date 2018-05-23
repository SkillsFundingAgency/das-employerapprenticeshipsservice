using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetStatistics;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.StatisticsControllerTests
{
    [TestFixture]
    public class WhenICallTheStatisticsEndPoint
    {
        private Mock<IMediator> _mediator;
        private StatisticsController _controller;
        private GetStatisticsResponse _response;
        private StatisticsViewModel _statisticsViewModel;


        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _statisticsViewModel = new StatisticsViewModel
            {
                TotalAccounts = 1,
                TotalPAYESchemes = 2,
                TotalActiveLegalEntities = 3,
                TotalSignedAgreements = 4,
                TotalPaymentsThisYear = 5
            };
            _response = new GetStatisticsResponse {Statistics = _statisticsViewModel};
        }

        [Test]
        public async Task ThenTheStatisticsAreReturned()
        {
            SetupMediatorResponse(true);

            var actual = await _controller.GetStatistics();
            
            Assert.IsNotNull(actual);
        }

        private void SetupMediatorResponse(bool withValidData)
        {
            if (!withValidData)
            {
                _response.Statistics = new StatisticsViewModel();
            }

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetStatisticsQuery>())).ReturnsAsync(_response);

            _controller = new StatisticsController(_mediator.Object);
        }

        [Test]
        public async Task ThenIfTheStatisticsAreNotAvailableReturnsNotFoundResult()
        {
            SetupMediatorResponse(false);
            var actual = await _controller.GetStatistics();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<NotFoundResult>(actual);
        }
    }
}

