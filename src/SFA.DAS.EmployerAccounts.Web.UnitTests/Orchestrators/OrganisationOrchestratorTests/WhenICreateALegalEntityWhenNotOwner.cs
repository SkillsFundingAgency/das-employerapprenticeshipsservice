using AutoMapper;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    class WhenICreateALegalEntityWhenNotOwner
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<IEncodingService> _encodingServiceMock;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        [SetUp()]
        public void Setup()
        {

            _mediator = new Mock<IMediator>();
            _mapper = new Mock<IMapper>();
            _encodingServiceMock = new Mock<IEncodingService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

            _orchestrator = new OrganisationOrchestrator(
                _mediator.Object,
                _mapper.Object,
                _cookieService.Object,
                _encodingServiceMock.Object);
        }

        [Test]
        public async Task ThenTheLegalEntityShouldNotBeCreated()
        {
            //Assign
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = "1",
                Name = "Test Corp",
                Code = "SD665734",
                Source = OrganisationType.CompaniesHouse,
                Address = "1, Test Street",
                IncorporatedDate = DateTime.Now.AddYears(-20),
                ExternalUserId = "3", // ??????
                LegalEntityStatus = "active"
            };

            _mediator.Setup(x => x.Send(It.IsAny<CreateLegalEntityCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act & Assert
            var response = await _orchestrator.CreateLegalEntity(request);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.Status);
        }
    }
}