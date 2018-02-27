using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransfersPage
    {
        private Web.Controllers.TransfersController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly GetTransferConnectionInvitationsQuery _query = new GetTransferConnectionInvitationsQuery();
        private readonly GetTransferConnectionInvitationsResponse _response = new GetTransferConnectionInvitationsResponse();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new Web.Controllers.TransfersController(Mapper.Instance, _mediator.Object);
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

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.TypeOf<TransferConnectionInvitationsViewModel>());
        }
    }
}