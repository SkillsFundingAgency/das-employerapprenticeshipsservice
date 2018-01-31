using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.Transfers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersControllerTests
{
    public class WhenIViewTheTransfersPage
    {
        private TransfersController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        private readonly GetTransferConnectionInvitationsQuery _query = new GetTransferConnectionInvitationsQuery();
        private readonly GetTransferConnectionInvitationsResponse _response = new GetTransferConnectionInvitationsResponse();
        private readonly TransferConnectionInvitationsViewModel _viewModel = new TransferConnectionInvitationsViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _mapper.Setup(m => m.Map<TransferConnectionInvitationsViewModel>(_response)).Returns(_viewModel);

            _controller = new TransfersController(_mapper.Object, _mediator.Object);
        }

        [Test]
        public async Task ThenAGetTransferConnectionInvitationsQueryShouldBeSent()
        {
            await _controller.Index(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheTransfersPage()
        {
            var result = await _controller.Index(_query) as ViewResult;
            var model = result?.Model as TransferConnectionInvitationsViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}