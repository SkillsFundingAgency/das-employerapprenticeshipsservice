using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Results;
using SFA.DAS.EmployerFinance.Authorisation;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Mappings;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationAuthorizationComponent
    {
        private TransferConnectionsController _controller;
        private GetTransferConnectionInvitationAuthorizationQuery _query;
        private GetTransferConnectionInvitationAuthorizationResponse _response;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureToggleService;

        [SetUp]
        public void Arrange()
        {
            _query = new GetTransferConnectionInvitationAuthorizationQuery();
            var authResult = new AuthorizationResult();
            authResult.AddError(new EmployerFeatureAgreementNotSigned());
            _response = new GetTransferConnectionInvitationAuthorizationResponse { AuthorizationResult = authResult, IsValidSender = true,TransferAllowancePercentage = .25m };
            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);
            _featureToggleService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();

            _controller = new TransferConnectionsController(null, _mapper, _mediator.Object, _featureToggleService.Object);
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