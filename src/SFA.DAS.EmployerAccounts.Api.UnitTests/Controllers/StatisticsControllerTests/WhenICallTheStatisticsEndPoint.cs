﻿using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetStatistics;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.StatisticsControllerTests
{
    [TestFixture]
    public class WhenICallTheStatisticsEndPoint
    {
        private StatisticsController _controller;
        private Mock<IMediator> _mediator;
        private GetStatisticsResponse _response;
        private Statistics _statistics;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();

            _statistics = new Statistics
            {
                TotalAccounts = 1,
                TotalPayeSchemes = 2,
                TotalLegalEntities = 3,
                TotalAgreements = 4
            };

            _response = new GetStatisticsResponse { Statistics = _statistics };

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetStatisticsQuery>())).ReturnsAsync(_response);

            _controller = new StatisticsController(_mediator.Object);
        }

        [Test]
        public async Task ThenShouldReturnStatistics()
        {
            var result = await _controller.GetStatistics() as OkNegotiatedContentResult<Statistics>; 

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.SameAs(_statistics));
        }
    }
}