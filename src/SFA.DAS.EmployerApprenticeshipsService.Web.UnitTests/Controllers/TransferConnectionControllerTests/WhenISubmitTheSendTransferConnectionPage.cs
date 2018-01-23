using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenISubmitTheSendTransferConnectionPage
    {
        private const long TransferConnectionId = 123;

        private TransferConnectionInvitationController _controller;
        private CreatedTransferConnectionInvitationViewModel _viewModel;
        private Mock<IMediator> _mediator;
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _controller = new TransferConnectionInvitationController(_mapper.Object, _mediator.Object);

            _viewModel = new CreatedTransferConnectionInvitationViewModel
            {
                Message = new SendTransferConnectionInvitationCommand
                {
                    TransferConnectionInvitationId = TransferConnectionId
                }
            };
        }

        [Test]
        public async Task ThenASendTransferConnectionCommandShouldBeSentIfIChoseOption1()
        {
           _viewModel.Choice = "Confirm";

            await _controller.Send(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSentTransferConnectionPageIfIChoseOption1()
        {
            _viewModel.Choice = "Confirm";

            var result = await _controller.Send(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Sent"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("TransferConnectionInvitationId", out var transferConnectionId), Is.True);
            Assert.That(transferConnectionId, Is.EqualTo(TransferConnectionId));
        }

        [Test]
        public async Task ThenASendTransferConnectionCommandShouldNotBeSentIfIChoseOption2()
        {
            _viewModel.Choice = "GoToTransfersPage";

            await _controller.Send(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Never);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheTransfersPageIfIChoseOption2()
        {
            _viewModel.Choice = "GoToTransfersPage";

            var result = await _controller.Send(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Index"));
            Assert.That(result.RouteValues.TryGetValue("controller", out var controllerName), Is.True);
            Assert.That(controllerName, Is.EqualTo("Transfer"));
        }
    }
}