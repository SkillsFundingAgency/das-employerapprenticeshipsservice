using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionControllerTests
{
    public class WhenIViewTheSentTransferConnectionPage
    {
        private TransferConnectionInvitationController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        private readonly GetSentTransferConnectionInvitationQuery _query = new GetSentTransferConnectionInvitationQuery();
        private readonly GetSentTransferConnectionInvitationResponse _response = new GetSentTransferConnectionInvitationResponse();
        private readonly SentTransferConnectionInvitationViewModel _viewModel = new SentTransferConnectionInvitationViewModel();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _mapper.Setup(m => m.Map<SentTransferConnectionInvitationViewModel>(_response)).Returns(_viewModel);

            _controller = new TransferConnectionInvitationController(_mapper.Object, _mediator.Object);
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
            var model = result?.Model as SentTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.SameAs(_viewModel));
        }
    }
}