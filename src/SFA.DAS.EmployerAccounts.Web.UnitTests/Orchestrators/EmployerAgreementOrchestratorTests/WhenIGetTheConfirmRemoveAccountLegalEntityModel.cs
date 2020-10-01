using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetTheConfirmRemoveAccountLegalEntityModel
    {
        private Mock<IMediator> _mediator;
        private Mock<IReferenceDataService> _referenceDataService;
        private EmployerAgreementOrchestrator _orchestrator;
        private Mock<IAccountRepository> _accountRepository;
        private const string ExpectedHahsedAccountId = "RT456";
        private const string ExpectedHashedAccountLegalEntityId = "RRTE56";
        private const string ExpectedUserId = "TYG68UY";
        private const string ExpectedName = "Test Name";

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntityRemoveRequest>()))
                .ReturnsAsync(new GetAccountLegalEntityRemoveResponse
                {
                    Name = ExpectedName,
                    CanBeRemoved = true,
                    HasSignedAgreement = true
                });

            _referenceDataService = new Mock<IReferenceDataService>();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, Mock.Of<IMapper>(), _referenceDataService.Object,
                _accountRepository.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetASingledOrgToRemove()
        {

            //Act
            await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAccountLegalEntityId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountLegalEntityRemoveRequest>(
                c => c.HashedAccountId.Equals(ExpectedHahsedAccountId)
                     && c.UserId.Equals(ExpectedUserId)
                     && c.HashedAccountLegalEntityId.Equals(ExpectedHashedAccountLegalEntityId)
                )), Times.Once);
        }


        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntityRemoveRequest>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAccountLegalEntityId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntityRemoveRequest>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAccountLegalEntityId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheMediatorCall()
        {
            //Act
            var actual = await _orchestrator.GetConfirmRemoveOrganisationViewModel(ExpectedHashedAccountLegalEntityId, ExpectedHahsedAccountId, ExpectedUserId);

            //Assert
            Assert.AreEqual(ExpectedHashedAccountLegalEntityId, actual.Data.HashedAccountLegalEntitytId);
            Assert.AreEqual(ExpectedHahsedAccountId, actual.Data.HashedAccountId);
            Assert.AreEqual(ExpectedName, actual.Data.Name);
        }

    }
}