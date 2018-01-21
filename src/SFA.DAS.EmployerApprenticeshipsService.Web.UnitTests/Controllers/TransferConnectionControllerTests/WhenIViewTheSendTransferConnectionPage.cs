using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnection;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenIViewTheSendTransferConnectionPage
    {
        private TransferConnectionController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly GetCreatedTransferConnectionQuery _query = new GetCreatedTransferConnectionQuery();
        private readonly CreatedTransferConnectionViewModel _viewModel = new CreatedTransferConnectionViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_viewModel);

            _controller = new TransferConnectionController(_mediator.Object);
        }

        [Test]
        public async Task ThenAGetCreatedTransferConnectionQueryShouldBeSent()
        {
            await _controller.Send(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheSendTransferConnectionPage()
        {
            var result = await _controller.Send(_query) as ViewResult;
            var model = result?.Model as CreatedTransferConnectionViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}