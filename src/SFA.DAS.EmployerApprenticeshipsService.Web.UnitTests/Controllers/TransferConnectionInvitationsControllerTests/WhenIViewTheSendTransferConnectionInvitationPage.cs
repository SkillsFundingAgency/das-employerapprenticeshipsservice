﻿using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Mappings;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIViewTheSendTransferConnectionInvitationPage
    {
        private TransferConnectionInvitationsController _controller;
        private IConfigurationProvider _configurationProvider;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private readonly SendTransferConnectionInvitationQuery _query = new SendTransferConnectionInvitationQuery();
        private readonly SendTransferConnectionInvitationResponse _response = new SendTransferConnectionInvitationResponse();

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
        public async Task ThenAGetCreatedTransferConnectionQueryShouldBeSent()
        {
            await _controller.Send(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeShownTheSendTransferConnectionInvitationPage()
        {
            var result = await _controller.Send(_query) as ViewResult;
            var model = result?.Model as SendTransferConnectionInvitationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
        }
    }
}