using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    public class WhenISubmitTheStartTransferConnectionInvitationPage
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ReceiverHashedAccountId = "XYZ987";

        private TransferConnectionInvitationsController _controller;
        private StartTransferConnectionInvitationViewModel _viewModel;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransferConnectionInvitationAccountQuery>()))
                .ReturnsAsync(new GetTransferConnectionInvitationAccountResponse());

            _controller = new TransferConnectionInvitationsController(_mapper.Object, _mediator.Object);

            _viewModel = new StartTransferConnectionInvitationViewModel
            {
                Message = new GetTransferConnectionInvitationAccountQuery
                {
                    SenderAccountHashedId = SenderHashedAccountId,
                    ReceiverAccountHashedId = ReceiverHashedAccountId
                }
            };
        }

        [Test]
        public async Task ThenAGetTransferConnectionInvitationAccountQueryShouldBeSent()
        {
            await _controller.Start(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSendTransferConnectionPage()
        {
            var result = await _controller.Start(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Send"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("SenderAccountHashedId", out var senderHashedAccountId), Is.True);
            Assert.That(senderHashedAccountId, Is.EqualTo(SenderHashedAccountId));
            Assert.That(result.RouteValues.TryGetValue("ReceiverAccountHashedId", out var receiverHashedAccountId), Is.True);
            Assert.That(receiverHashedAccountId, Is.EqualTo(ReceiverHashedAccountId));
        }
    }
}