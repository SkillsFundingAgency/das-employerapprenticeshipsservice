using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenISearchForAnOrganisation
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOrganisationsRequest>()))
                .ReturnsAsync(new GetOrganisationsResponse { Organisations = new PagedResponse<Organisation>()});

            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

            _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _cookieService.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetTheOrganisationResult()
        {
            //Arrange
            var searchTerm = "Test Org";
            var pageNumber = 2;

            //Act
            await _orchestrator.SearchOrganisation(searchTerm, pageNumber);


            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetOrganisationsRequest>(c => c.SearchTerm.Equals(searchTerm) && c.PageNumber == pageNumber)), Times.Once);
        }

        [Test]
        public async Task ThenTheOrganisationListIsReturnedInTheResult()
        {
            //Arrange
            var searchTerm = "Test Org";
            var pageNumber = 1;

            //Act
            var actual = await _orchestrator.SearchOrganisation(searchTerm, pageNumber);

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
            var actual = await _orchestrator.SearchOrganisation("Test", 1);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest,actual.Status);
        }
    }
}
