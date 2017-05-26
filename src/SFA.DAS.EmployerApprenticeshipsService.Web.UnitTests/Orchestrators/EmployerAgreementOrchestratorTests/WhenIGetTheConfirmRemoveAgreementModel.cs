using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetTheConfirmRemoveAgreementModel
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private EmployerAgreementOrchestrator _orchestrator;

        private const string ExpectedHahsedAccountId = "RT456";
        private const string ExpectedHashedAgreementId = "RRTE56";
        private const string ExpectedUserId = "TYG68UY";
        private const string ExpectedName = "Test Name";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountEmployerAgreementRemoveRequest>()))
                .ReturnsAsync(new GetAccountEmployerAgreementRemoveResponse
                {
                    Agreement = new RemoveEmployerAgreementView
                    {
                        Name = ExpectedName,
                        CanBeRemoved = false,
                        HashedAccountId = ExpectedHahsedAccountId,
                        HashedAgreementId = ExpectedHashedAgreementId,
                        Id = 123444
                    }
                    
                });

            _logger = new Mock<ILogger>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, _configuration);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetASingledOrgToRemove()
        {

            //Act
            await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAgreementId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountEmployerAgreementRemoveRequest>(
                c => c.HashedAccountId.Equals(ExpectedHahsedAccountId)
                     && c.UserId.Equals(ExpectedUserId)
                     && c.HashedAgreementId.Equals(ExpectedHashedAgreementId)
                )), Times.Once);
        }


        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountEmployerAgreementRemoveRequest>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAgreementId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountEmployerAgreementRemoveRequest>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAgreementId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheMediatorCall()
        {
            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAgreementId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(ExpectedHashedAgreementId,actual.Data.HashedAgreementId);
            Assert.AreEqual(ExpectedHahsedAccountId,actual.Data.HashedAccountId);
            Assert.AreEqual(ExpectedName,actual.Data.Name);
        }

    }
}