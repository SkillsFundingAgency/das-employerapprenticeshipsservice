using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.SearchOrganisationOrchestratorTests
{
    public class WhenISearchForAnOrganisation
    {
        private SearchOrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;



        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationsRequest>()))
                .ReturnsAsync(new GetOrganisationsResponse { Organisations = new PagedResponse<OrganisationName> { Data = new List<OrganisationName>() } });

            _orchestrator = new SearchOrganisationOrchestrator(_mediator.Object, _cookieService.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetTheOrganisationResult()
        {
            //Arrange
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var organisationType = OrganisationType.Charities;

            //Act
            await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType, null, null);


            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber && c.OrganisationType == organisationType)), Times.Once);
        }

        [Test]
        public async Task ThenTheOrganisationListIsReturnedInTheResult()
        {
            //Arrange
            var searchTerm = "Test Org";
            var pageNumber = 1;
            var organisationType = OrganisationType.CompaniesHouse;

            //Act
            var actual = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType, null, null);

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<SearchOrganisationResultsViewModel>>(actual);
        }

        [Test]
        public async Task ThenAnInvalidRequestExceptionIsHandledTheOrchestratorResponseIsSetToBadRequest()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationsRequest>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> {{"", ""}}));

            //Act
            var actual = await _orchestrator.SearchOrganisation("Test", 1, OrganisationType.Charities, null, null);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest,actual.Status);
        }

        [Test]
        public async Task ThenIfAnOrganisationIsNotAlreadyAddedToTheAccountThenItIsSelectable()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "test";
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var expectedLegalEntitiesResponse = new GetAccountLegalEntitiesResponse
            {
                Entites = new LegalEntities { LegalEntityList = new List<LegalEntity> { new LegalEntity { Source = (byte)OrganisationType.CompaniesHouse, Code = "zzz999" } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(y => y.HashedLegalEntityId == hashedAccountId && y.UserId == userId))).ReturnsAsync(expectedLegalEntitiesResponse);
            var expectedSearchResults = new GetOrganisationsResponse
            {
                Organisations = new PagedResponse<OrganisationName> { Data = new List<OrganisationName> { new OrganisationName { Type = OrganisationType.CompaniesHouse, Code = "123456789", Address = new Address() } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber))).ReturnsAsync(expectedSearchResults);

            //Act
            var result = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, null, hashedAccountId, userId);

            //Assert
            Assert.IsFalse(result.Data.Results.Data.Single().AddedToAccount);
        }

        [Test]
        public async Task ThenIfACompanyIsAlreadyAddedToTheAccountThenItIsNotSelectable()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "test";
            var companyCode = "zzz9435";
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var expectedLegalEntitiesResponse = new GetAccountLegalEntitiesResponse
            {
                Entites = new LegalEntities { LegalEntityList = new List<LegalEntity> { new LegalEntity { Source = (byte)OrganisationType.CompaniesHouse, Code = companyCode } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(y => y.HashedLegalEntityId == hashedAccountId && y.UserId == userId))).ReturnsAsync(expectedLegalEntitiesResponse);
            var expectedSearchResults = new GetOrganisationsResponse
            {
                Organisations = new PagedResponse<OrganisationName> { Data = new List<OrganisationName> { new OrganisationName { Type = OrganisationType.CompaniesHouse, Code = companyCode, Address = new Address() } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber))).ReturnsAsync(expectedSearchResults);

            //Act
            var result = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, null, hashedAccountId, userId);

            //Assert
            Assert.IsTrue(result.Data.Results.Data.Single().AddedToAccount);
        }

        [Test]
        public async Task ThenIfACharityIsAlreadyAddedToTheAccountThenItIsNotSelectable()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "test";
            var charityCode = "zzz9435";
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var expectedLegalEntitiesResponse = new GetAccountLegalEntitiesResponse
            {
                Entites = new LegalEntities { LegalEntityList = new List<LegalEntity> { new LegalEntity { Source = (byte)OrganisationType.Charities, Code = charityCode } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(y => y.HashedLegalEntityId == hashedAccountId && y.UserId == userId))).ReturnsAsync(expectedLegalEntitiesResponse);
            var expectedSearchResults = new GetOrganisationsResponse
            {
                Organisations = new PagedResponse<OrganisationName> { Data = new List<OrganisationName> { new OrganisationName { Type = OrganisationType.Charities, Code = charityCode, Address = new Address() } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber))).ReturnsAsync(expectedSearchResults);

            //Act
            var result = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, null, hashedAccountId, userId);

            //Assert
            Assert.IsTrue(result.Data.Results.Data.Single().AddedToAccount);
        }

        [Test]
        public async Task ThenIfAPublicSectorOrganisationIsAlreadyAddedToTheAccountThenItIsNotSelectable()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "test";
            var organisationName = "Org Name";
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var expectedLegalEntitiesResponse = new GetAccountLegalEntitiesResponse
            {
                Entites = new LegalEntities { LegalEntityList = new List<LegalEntity> { new LegalEntity { Source = (byte)OrganisationType.PublicBodies, Name = organisationName } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(y => y.HashedLegalEntityId == hashedAccountId && y.UserId == userId))).ReturnsAsync(expectedLegalEntitiesResponse);
            var expectedSearchResults = new GetOrganisationsResponse
            {
                Organisations = new PagedResponse<OrganisationName> { Data = new List<OrganisationName> { new OrganisationName { Type = OrganisationType.PublicBodies, Name = organisationName, Address = new Address() } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber))).ReturnsAsync(expectedSearchResults);

            //Act
            var result = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, null, hashedAccountId, userId);

            //Assert
            Assert.IsTrue(result.Data.Results.Data.Single().AddedToAccount);
        }

        [Test]
        public async Task ThenIfAnOtherOrganisationIsAlreadyAddedToTheAccountThenItIsNotSelectable()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "test";
            var organisationName = "Org Name";
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var expectedLegalEntitiesResponse = new GetAccountLegalEntitiesResponse
            {
                Entites = new LegalEntities { LegalEntityList = new List<LegalEntity> { new LegalEntity { Source = (byte)OrganisationType.Other, Name = organisationName } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountLegalEntitiesRequest>(y => y.HashedLegalEntityId == hashedAccountId && y.UserId == userId))).ReturnsAsync(expectedLegalEntitiesResponse);
            var expectedSearchResults = new GetOrganisationsResponse
            {
                Organisations = new PagedResponse<OrganisationName> { Data = new List<OrganisationName> { new OrganisationName { Type = OrganisationType.Other, Name = organisationName, Address = new Address() } } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber))).ReturnsAsync(expectedSearchResults);

            //Act
            var result = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, null, hashedAccountId, userId);

            //Assert
            Assert.IsTrue(result.Data.Results.Data.Single().AddedToAccount);
        }

        [Test]
        public async Task ThenIfThereIsNoDasAccountThenExistingOrganisationsArentChecked()
        {
            //Act
            await _orchestrator.SearchOrganisation("Test Org", 1, null, null, null);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()), Times.Never);
        }
    }
}
