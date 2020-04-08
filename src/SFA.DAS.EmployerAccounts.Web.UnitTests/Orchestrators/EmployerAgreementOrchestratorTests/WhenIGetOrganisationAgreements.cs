using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetOrganisationAgreements
    {
        private Mock<IMediator> _mediator;
        private Mock<IReferenceDataService> _referenceDataService;
        private Mock<IMapper> _mapper;
        private EmployerAgreementOrchestrator _orchestrator;        

        public string AccountLegalEntityHashedId = "2K7J94";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mapper = new Mock<IMapper>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationAgreementsRequest>()))
                .ReturnsAsync(new GetOrganisationAgreementsResponse
                {
                   OrganisationAgreements = new OrganisationAgreement
                   {
                       Address = "Address",
                       Name = "Name"
                   }
                });;

            var organisationAgreementViewModel = new OrganisationAgreementViewModel
            {
                Address = "Address",
                Name = "Name"
            };

            _referenceDataService = new Mock<IReferenceDataService>();
            _mapper.Setup(m => m.Map<OrganisationAgreement, OrganisationAgreementViewModel>(It.IsAny<OrganisationAgreement>())).Returns(organisationAgreementViewModel);
            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _mapper.Object, _referenceDataService.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithAccountLegalEntityId()
        {

            //Act
            await _orchestrator.GetOrganisationAgreements(AccountLegalEntityHashedId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetOrganisationAgreementsRequest>(
                                c => c.AccountLegalEntityHashedId.Equals(AccountLegalEntityHashedId))));
        }

        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationAgreementsRequest>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.GetOrganisationAgreements(AccountLegalEntityHashedId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationAgreementsRequest>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetOrganisationAgreements(AccountLegalEntityHashedId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheMediatorCall()
        {
            //Act
            var actual = await _orchestrator.GetOrganisationAgreements(AccountLegalEntityHashedId);

            //Assert
            //TO DO 
            Assert.IsTrue(actual.Data.Address.Equals("Address"));
        }
    }
}
