﻿using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Queries.GetTransferRequests;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Mappings;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferRequestsComponent
    {
        private TransfersController _controller;
        private GetTransferRequestsQuery _query;
        private GetTransferRequestsResponse _response;
        private Mock<ILog> _logger;
        private IConfigurationProvider _mapperConfig;
        private IMapper _mapper;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _query = new GetTransferRequestsQuery();

            _response = new GetTransferRequestsResponse
            {
                TransferRequests = new List<TransferRequestDto>()
            };

            _logger = new Mock<ILog>();
            _mapperConfig = new MapperConfiguration(c => c.AddProfile<TransferMappings>());
            _mapper = _mapperConfig.CreateMapper();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(_query)).ReturnsAsync(_response);

            _controller = new TransfersController(_logger.Object, _mapper, _mediator.Object);
        }

        [Test]
        public void ThenAGetTransferRequestsQueryShouldBeSent()
        {
            _controller.TransferRequests(_query);

            _mediator.Verify(m => m.SendAsync(_query), Times.Once);
        }

        [Test]
        public void ThenIShouldBeShownTheTransferRequestsComponent()
        {
            var result = _controller.TransferRequests(_query) as PartialViewResult;
            var model = result?.Model as TransferRequestsViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(model, Is.Not.Null);
            Assert.That(model.TransferRequests, Is.EqualTo(_response.TransferRequests));
        }

        [Test]
        public void ThenExceptionShouldBeLoggedWhenExceptionIsThrown()
        {
            _mediator.Setup(m => m.SendAsync(_query)).Throws<Exception>();
            
            var result = _controller.TransferRequests(_query) as EmptyResult;

            Assert.That(result, Is.Not.Null);

            _logger.Verify(l => l.Warn(It.IsAny<Exception>(), "Failed to get transfer requests"), Times.Once);
        }
    }
}