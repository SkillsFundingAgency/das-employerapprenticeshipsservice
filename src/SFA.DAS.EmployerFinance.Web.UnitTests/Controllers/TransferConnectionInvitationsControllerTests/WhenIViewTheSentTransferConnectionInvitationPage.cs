using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Mappings;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIViewTheSentTransferConnectionInvitationPage
    {
        private TransferConnectionInvitationsController _controller;
        private IConfigurationProvider _configurationProvider;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private readonly GetSentTransferConnectionInvitationQuery _query = new GetSentTransferConnectionInvitationQuery();
        private readonly GetSentTransferConnectionInvitationResponse _response = new GetSentTransferConnectionInvitationResponse();

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
        public async Task ThenAGetSentTransferConnectionQueryShouldBeSent()
        {
            await _controller.Sent(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheSentTransferConnectionInvitationPage()
        {
            var result = await _controller.Sent(_query) as ViewResult;
            var model = result?.Model as SentTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
        }
    }
}