using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationsComponent
    {
        private TransfersController _controller;
        private GetTransferConnectionInvitationsQuery _query;
        private GetTransferConnectionInvitationsResponse _response;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _query = new GetTransferConnectionInvitationsQuery();

            _response = new GetTransferConnectionInvitationsResponse
            {
                TransferConnectionInvitations = new List<TransferConnectionInvitationDto>()
            };

            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new TransfersController(null, _mapper, _mediator.Object,null);
        }

        [Test]
        public void ThenAGetTransferConnectionInvitationsQueryShouldBeSent()
        {
            _controller.TransferConnectionInvitations(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public void ThenIShouldBeShownTheTransferConnectionInvitationsComponent()
        {
            var result = _controller.TransferConnectionInvitations(_query) as PartialViewResult;
            var model = result?.Model as TransferConnectionInvitationsViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model.TransferConnectionInvitations, Is.EqualTo(_response.TransferConnectionInvitations));
        }
    }
}