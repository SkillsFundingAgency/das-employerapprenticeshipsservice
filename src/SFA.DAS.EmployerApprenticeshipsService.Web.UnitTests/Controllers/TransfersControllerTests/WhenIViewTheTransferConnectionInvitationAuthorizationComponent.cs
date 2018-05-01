using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Mappings;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationAuthorizationComponent
    {
        private TransfersController _controller;
        private GetTransferConnectionInvitationAuthorizationQuery _query;
        private GetTransferConnectionInvitationAuthorizationResponse _response;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _query = new GetTransferConnectionInvitationAuthorizationQuery();
            _response = new GetTransferConnectionInvitationAuthorizationResponse { AuthorizationResult = AuthorizationResult.FeatureAgreementNotSigned, IsValidSender = true };
            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new TransfersController(_mapper, _mediator.Object);
        }

        [Test]
        public void ThenAGetTransferConnectionInvitationAuthorizationQueryShouldBeSent()
        {
            _controller.TransferConnectionInvitationAuthorization(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public void ThenIShouldBeShownTheTransferConnectionInvitationAuthorizationComponent()
        {
            var result = _controller.TransferConnectionInvitationAuthorization(_query) as PartialViewResult;
            var model = result?.Model as TransferConnectionInvitationAuthorizationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model.AuthorizationResult, Is.EqualTo(_response.AuthorizationResult));
            Assert.That(model.IsValidSender, Is.EqualTo(_response.IsValidSender));
        }
    }
}