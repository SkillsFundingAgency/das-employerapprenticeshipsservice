using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIViewTheSentTransferConnectionInvitationPage
    {
        private TransferConnectionInvitationsController _controller;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly GetSentTransferConnectionInvitationQuery _query = new GetSentTransferConnectionInvitationQuery();
        private readonly GetSentTransferConnectionInvitationResponse _response = new GetSentTransferConnectionInvitationResponse();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new TransferConnectionInvitationsController(Mapper.Instance, _mediator.Object);
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
        }
    }
}