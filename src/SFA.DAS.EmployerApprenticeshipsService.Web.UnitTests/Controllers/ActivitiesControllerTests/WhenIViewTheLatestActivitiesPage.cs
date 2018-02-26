using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.Activities;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.ActivitiesControllerTests
{
    [TestFixture]
    public class WhenIViewTheLatestActivitiesPage
    {
        private ActivitiesController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        private readonly GetLatestActivitiesQuery _query = new GetLatestActivitiesQuery();
        private readonly GetLatestActivitiesResponse _response = new GetLatestActivitiesResponse();
        private readonly LatestActivitiesViewModel _viewModel = new LatestActivitiesViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _mapper.Setup(m => m.Map<LatestActivitiesViewModel>(_response)).Returns(_viewModel);

            _controller = new ActivitiesController(_mapper.Object, _mediator.Object);
        }

        [Test]
        public void ThenAGetLatestActivitiesQueryShouldBeSent()
        {
            _controller.Latest(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public void ThenIShouldBeShownTheActivitiesPage()
        {
            var result = _controller.Latest(_query) as PartialViewResult;
            var model = result?.Model as LatestActivitiesViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}