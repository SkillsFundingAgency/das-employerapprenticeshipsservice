using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.TransfersControllerTests
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
            _response = new GetTransferConnectionInvitationAuthorizationResponse { AuthorizationResult = AuthorizationResult.FeatureAgreementNotSigned, IsValidSender = true,TransferAllowancePercentage = .25m };
            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new TransfersController(null, _mapper, _mediator.Object);
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

        [Test]
        public void ThenIShouldBeShownTheCorrectTransferAllowancePercentage()
        {
            //Act
            var result = _controller.TransferConnectionInvitationAuthorization(_query) as PartialViewResult;
            var model = result?.Model as TransferConnectionInvitationAuthorizationViewModel;

            //Assert
            Assert.AreEqual(25m, model.TransferAllowancePercentage);
        }
    }
}