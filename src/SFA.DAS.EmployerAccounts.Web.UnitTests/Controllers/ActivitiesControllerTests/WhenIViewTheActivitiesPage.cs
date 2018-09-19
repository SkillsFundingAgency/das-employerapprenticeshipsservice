using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Activities;
using SFA.DAS.EmployerAccounts.Queries.GetActivities;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.ActivitiesControllerTests
{
    [TestFixture]
    public class WhenIViewTheActivitiesPage
    {
        private ActivitiesController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        private readonly Mock<ILog> _logger = new Mock<ILog>();
        private readonly GetActivitiesQuery _query = new GetActivitiesQuery();
        private readonly GetActivitiesResponse _response = new GetActivitiesResponse();
        private readonly ActivitiesViewModel _viewModel = new ActivitiesViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _mapper.Setup(m => m.Map<ActivitiesViewModel>(_response)).Returns(_viewModel);

            _controller = new ActivitiesController(_mapper.Object, _mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenAGetActivitiesQueryShouldBeSent()
        {
            await _controller.Index(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheActivitiesPage()
        {
            var result = await _controller.Index(_query) as ViewResult;
            var model = result?.Model as ActivitiesViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}