﻿using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Mappings;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationsComponent
    {
        private TransferConnectionsController _controller;
        private GetTransferConnectionInvitationsQuery _query;
        private GetTransferConnectionInvitationsResponse _response;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureToggleService;

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
            _featureToggleService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();

            _controller = new TransferConnectionsController(null, _mapper, _mediator.Object, _featureToggleService.Object);
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