using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    public class WhenIViewTheSendTransferConnectionInvitationPage
    {
        private TransferConnectionInvitationsController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        private readonly GetTransferConnectionInvitationAccountQuery _query = new GetTransferConnectionInvitationAccountQuery();
        private readonly GetTransferConnectionInvitationAccountResponse _response = new GetTransferConnectionInvitationAccountResponse();
        private readonly SendTransferConnectionInvitationViewModel _viewModel = new SendTransferConnectionInvitationViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _mapper.Setup(m => m.Map<SendTransferConnectionInvitationViewModel>(_response)).Returns(_viewModel);

            _controller = new TransferConnectionInvitationsController(_mapper.Object, _mediator.Object);
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
            var model = result?.Model as SendTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}