using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnection;
using SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnection;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenISubmitTheSendTransferConnectionPage
    {
        private const long TransferConnectionId = 123;

        private TransferConnectionController _controller;
        private CreatedTransferConnectionViewModel _viewModel;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _controller = new TransferConnectionController(_mediator.Object);

            _viewModel = new CreatedTransferConnectionViewModel
            {
                Message = new SendTransferConnectionCommand
                {
                    TransferConnectionId = TransferConnectionId
                }
            };
        }

        [Test]
        public async Task ThenASendTransferConnectionCommandShouldBeSentIfIChoseOption1()
        {
           _viewModel.Choice = 1;

            await _controller.Send(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSentTransferConnectionPageIfIChoseOption1()
        {
            _viewModel.Choice = 1;

            var result = await _controller.Send(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Sent"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("TransferConnectionId", out var transferConnectionId), Is.True);
            Assert.That(transferConnectionId, Is.EqualTo(TransferConnectionId));
        }

        [Test]
        public async Task ThenASendTransferConnectionCommandShouldNotBeSentIfIChoseOption2()
        {
            _viewModel.Choice = 2;

            await _controller.Send(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Never);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheTransfersPageIfIChoseOption2()
        {
            _viewModel.Choice = 2;

            var result = await _controller.Send(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("Transfer"));
        }
    }
}