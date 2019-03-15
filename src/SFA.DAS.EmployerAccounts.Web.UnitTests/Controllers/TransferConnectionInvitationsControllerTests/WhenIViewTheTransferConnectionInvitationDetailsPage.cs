using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationDetailsPage
    {
        private TransferConnectionInvitationsController _controller;
        private IConfigurationProvider _configurationProvider;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private readonly GetTransferConnectionInvitationQuery _query = new GetTransferConnectionInvitationQuery();
        private readonly GetTransferConnectionInvitationResponse _response = new GetTransferConnectionInvitationResponse();

        [SetUp]
        public void Arrange()
        {
            _configurationProvider = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _configurationProvider.CreateMapper();
            _mediator = new Mock<IMediator>();

            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new TransferConnectionInvitationsController(_mapper, _mediator.Object);
        }

        [Test]
        public async Task ThenAGetTransferConnectionQueryShouldBeSent()
        {
            await _controller.Details(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheTransferConnectionDetailsPage()
        {
            var result = await _controller.Details(_query) as ViewResult;
            var model = result?.Model as TransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
        }
    }
}