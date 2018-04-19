using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenISubmitTheStartTransferConnectionInvitationPage
    {
        private const string ReceiverAccountPublicHashedId = "XYZ987";

        private TransferConnectionInvitationsController _controller;
        private StartTransferConnectionInvitationViewModel _viewModel;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<SendTransferConnectionInvitationQuery>())).ReturnsAsync(new SendTransferConnectionInvitationResponse());

            _controller = new TransferConnectionInvitationsController(null, _mediator.Object);

            _viewModel = new StartTransferConnectionInvitationViewModel
            {
                SendTransferConnectionInvitationQuery = new SendTransferConnectionInvitationQuery
                {
                    ReceiverAccountPublicHashedId = ReceiverAccountPublicHashedId
                }
            };
        }

        [Test]
        public async Task ThenAGetTransferConnectionInvitationAccountQueryShouldBeSent()
        {
            await _controller.Start(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.SendTransferConnectionInvitationQuery), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSendTransferConnectionInvitationPage()
        {
            var result = await _controller.Start(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Send"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("ReceiverAccountPublicHashedId", out var receiverPublicHashedAccountId), Is.True);
            Assert.That(receiverPublicHashedAccountId, Is.EqualTo(ReceiverAccountPublicHashedId));
        }
    }
}