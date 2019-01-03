using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetTheConfirmRemoveAgreementModel
    {
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IReferenceDataService> _referenceDataService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private EmployerAgreementOrchestrator _orchestrator;

        private const string ExpectedHahsedAccountId = "RT456";
        //private const string ExpectedHashedAgreementId = "RRTE56";
        private const string ExpectedAccountLegalEntityPublicHashedId = "AFG99";
        private const string ExpectedUserId = "TYG68UY";
        private const string ExpectedName = "Test Name";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationRemoveRequest>()))
                .ReturnsAsync(new GetOrganisationRemoveResponse
                {
                    Organisation = new RemoveOrganisationView
                    {
                        Name = ExpectedName,
                        CanBeRemoved = false,
                        HashedAccountId = ExpectedHahsedAccountId,
                        AccountLegalEntityPublicHashedId = ExpectedAccountLegalEntityPublicHashedId
                    }

                });

            _logger = new Mock<ILog>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();
            _referenceDataService = new Mock<IReferenceDataService>();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, Mock.Of<IMapper>(), _configuration, _referenceDataService.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetASingledOrgToRemove()
        {

            //Act
            await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedAccountLegalEntityPublicHashedId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetOrganisationRemoveRequest>(
                c => c.HashedAccountId.Equals(ExpectedHahsedAccountId)
                     && c.UserId.Equals(ExpectedUserId)
                     && c.AccountLegalEntityPublicHashedId.Equals(ExpectedAccountLegalEntityPublicHashedId)
                )), Times.Once);
        }


        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationRemoveRequest>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedAccountLegalEntityPublicHashedId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationRemoveRequest>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedAccountLegalEntityPublicHashedId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheMediatorCall()
        {
            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedAccountLegalEntityPublicHashedId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(ExpectedAccountLegalEntityPublicHashedId, actual.Data.AccountLegalEntityPublicHashedId);
            Assert.AreEqual(ExpectedHahsedAccountId, actual.Data.HashedAccountId);
            Assert.AreEqual(ExpectedName, actual.Data.Name);
        }

    }
}