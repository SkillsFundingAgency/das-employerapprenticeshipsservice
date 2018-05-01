﻿using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetAListOfAgreementsToRemove
    {
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private EmployerAgreementOrchestrator _orchestrator;

        private const string ExpectedHahsedAccountId = "RT456";
        private const string ExpectedUserId = "TYG68UY";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountEmployerAgreementsRemoveRequest>()))
                .ReturnsAsync(new GetAccountEmployerAgreementsRemoveResponse
                {
                    Agreements = new List<RemoveEmployerAgreementView>
                    {
                        new RemoveEmployerAgreementView { Name = "Test Name", CanBeRemoved = false}
                    }
                });

            _logger = new Mock<ILog>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, Mock.Of<IMapper>(), _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheListOfOrganisations()
        {

            //Act
            await _orchestrator.GetLegalAgreementsToRemove(ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountEmployerAgreementsRemoveRequest>(
                                c => c.HashedAccountId.Equals(ExpectedHahsedAccountId)
                                && c.UserId.Equals(ExpectedUserId))), Times.Once);
        }


        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountEmployerAgreementsRemoveRequest>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.GetLegalAgreementsToRemove(ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountEmployerAgreementsRemoveRequest>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetLegalAgreementsToRemove(ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheMediatorCall()
        {
            //Act
            var actual = await _orchestrator.GetLegalAgreementsToRemove(ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.IsTrue(actual.Data.Agreements.Any());
        }

    }
}
