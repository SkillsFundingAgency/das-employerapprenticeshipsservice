﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;
using System.Linq;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.TestCommon;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetOrganisationAgreements
    {
        private Mock<IMediator> _mediator;
        private Mock<IReferenceDataService> _referenceDataService;
        private Mock<IMapper> _mapper;
        private EmployerAgreementOrchestrator _orchestrator;
        private EmployerAgreementBuilder EmployerAgreementBuilder { get; }
        public string AccountLegalEntityHashedId = "2K7J94";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mapper = new Mock<IMapper>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationAgreementsRequest>()))
                .ReturnsAsync(new GetOrganisationAgreementsResponse
                {
                    Agreements = new List<EmployerAgreementDto>()
                    {
                        new EmployerAgreementDto { SignedDate = DateTime.UtcNow }
                    }
                });             

            var organisationAgreementViewModel = new List<OrganisationAgreementViewModel>()
            {
                new OrganisationAgreementViewModel { SignedDate = DateTime.UtcNow }
            };

            _referenceDataService = new Mock<IReferenceDataService>();
            _mapper.Setup(m => m.Map<ICollection<EmployerAgreementDto>, ICollection<OrganisationAgreementViewModel>>(It.IsAny<ICollection<EmployerAgreementDto>>())).Returns(organisationAgreementViewModel);
            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _mapper.Object, _referenceDataService.Object, new Lazy<EmployerAccountsDbContext>(() => EmployerAgreementBuilder.EmployerAccountDbContext));
        }

        [Test]
        public async Task ThenTheRequestForAllOrganisationAgreementsIsMadeForAccountLegalEntity()
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
        public async Task ThenTheValuesAreReturnedInTheResponseFromTheRequestForAllOrganisationAgreementsIsMadeForAccountLegalEntity()
        {
            //Act
            var actual = await _orchestrator.GetOrganisationAgreements(AccountLegalEntityHashedId);

            //Assert
            Assert.IsNotNull(actual.Data.Any());            
        }
    }
}
