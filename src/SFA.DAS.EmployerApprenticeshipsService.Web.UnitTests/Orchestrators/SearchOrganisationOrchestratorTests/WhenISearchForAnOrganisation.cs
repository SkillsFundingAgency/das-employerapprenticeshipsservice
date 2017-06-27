using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
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
                .ReturnsAsync(new GetOrganisationsResponse { Organisations = new PagedResponse<Organisation> { Data = new List<Organisation>() } });

            _orchestrator = new SearchOrganisationOrchestrator(_mediator.Object, _cookieService.Object);
        }

        [Test]public async Task ThenTheMediatorIsCalledToGetTheOrganisationResult()
        {
            //Arrange
            var searchTerm = "Test Org";
            var pageNumber = 2;
            var organisationType = OrganisationType.Charities;

            //Act
            await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType);


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
            var actual = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType);

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<SearchOrganisationViewModel>>(actual);
        }

        [Test]
        public async Task ThenAnInvalidRequestExceptionIsHandledTheOrchestratorResponseIsSetToBadRequest()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationsRequest>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> {{"", ""}}));

            //Act
            var actual = await _orchestrator.SearchOrganisation("Test", 1, OrganisationType.Charities);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest,actual.Status);
        }
    }
}
