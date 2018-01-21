using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenIViewTheSentTransferConnectionPage
    {
        private TransferConnectionController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly GetSentTransferConnectionQuery _query = new GetSentTransferConnectionQuery();
        private readonly SentTransferConnectionViewModel _viewModel = new SentTransferConnectionViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_viewModel);

            _controller = new TransferConnectionController(_mediator.Object);
        }

        [Test]
        public async Task ThenAGetSentTransferConnectionQueryShouldBeSent()
        {
            await _controller.Sent(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheSentTransferConnectionPage()
        {
            var result = await _controller.Sent(_query) as ViewResult;
            var model = result?.Model as SentTransferConnectionViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}