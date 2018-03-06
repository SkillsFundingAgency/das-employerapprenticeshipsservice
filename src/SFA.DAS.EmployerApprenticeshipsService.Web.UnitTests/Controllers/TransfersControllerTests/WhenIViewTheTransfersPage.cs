using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.Mappings;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransfersPage
    {
        private Web.Controllers.TransfersController _controller;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private readonly GetTransferConnectionInvitationsQuery _query = new GetTransferConnectionInvitationsQuery();
        private readonly GetTransferConnectionInvitationsResponse _response = new GetTransferConnectionInvitationsResponse();

        [SetUp]
        public void Arrange()
        {
            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferConnectionInvitationMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new Web.Controllers.TransfersController(_mapper, _mediator.Object);
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