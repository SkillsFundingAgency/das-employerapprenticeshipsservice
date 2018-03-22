using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionRoles;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Mappings;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationsPage
    {
        private TransferConnectionInvitationsController _controller;
        private IConfigurationProvider _configurationProvider;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private readonly GetTransferConnectionRolesResponse _response = new GetTransferConnectionRolesResponse();

        [SetUp]
        public void Arrange()
        {
            _configurationProvider = new MapperConfiguration(c => c.AddProfile<TransferConnectionInvitationMappings>());
            _mapper = _configurationProvider.CreateMapper();
            _mediator = new Mock<IMediator>();

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransferConnectionRolesQuery>())).ReturnsAsync(_response);
            _controller = new TransferConnectionInvitationsController(_mapper, _mediator.Object);
        }

        [Test]
        public Task ThenIShouldBeShownTheTransferConnectionsPage()
        {
            return CheckModelProperty(false, false, model => Assert.IsNotNull(model));
        }

        [Test]
        public Task ThenIsReceiverShouldBeFalseWhenAccountIsNotReceiver()
        {
            return CheckModelProperty(false, false, model => Assert.IsFalse(model.IsPendingReceiver));
        }

        [Test]
        public Task ThenIsReceiverShouldBeTrueWhenAccountHasOnlyPendingInvitations()
        {
            return CheckModelProperty(false, true, model => Assert.IsTrue(model.IsReceiver));
        }

        [Test]
        public Task ThenHasApprovedInvitationsShouldBeFalseWhenAccountHasOnlyPendingInvitations()
        {
            return CheckModelProperty(false, true, model => Assert.IsFalse(model.IsApprovedReceiver));
        }

        [Test]
        public Task ThenIsReceiverShouldBeTrueWhenAccountHasOnlyActiveInvitations()
        {
            return CheckModelProperty(true, false, model => Assert.IsTrue(model.IsReceiver));
        }

        [Test]
        public Task ThenHasApprovedInvitationsShouldBeTrueWhenAccountHasOnlyActiveInvitations()
        {
            return CheckModelProperty(true, false, model => Assert.IsTrue(model.IsApprovedReceiver));
        }

        [Test]
        public Task ThenIsReceiverShouldBeTrueWhenAccountHasBothPendingAndActiveInvitations()
        {
            return CheckModelProperty(true, true, model => Assert.IsTrue(model.IsReceiver));
        }

        [Test]
        public Task ThenHasApprovedInvitationsShouldBeTrueWhenAccountHasBothPendingAndActiveInvitations()
        {
            return CheckModelProperty(true, true, model => Assert.IsTrue(model.IsApprovedReceiver));
        }

        private async Task CheckModelProperty(bool isApprovedReceiver, bool isPendingreceiver, Action<TransferConnectionRolesViewModel> check)
        {
            _response.IsApprovedReceiver = isApprovedReceiver;
            _response.IsPendingReceiver = isPendingreceiver;

            var result = await _controller.Index(new GetTransferConnectionRolesQuery()) as ViewResult;
            var model = result?.Model as TransferConnectionRolesViewModel;

            check(model);
        }
    }
}