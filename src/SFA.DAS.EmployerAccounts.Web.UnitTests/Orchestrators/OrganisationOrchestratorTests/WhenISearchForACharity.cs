using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.ReferenceData;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetCharity;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Hashing;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenISearchForACharity
    {
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        private GetCharityQueryResponse _expected;
        private Mock<IAccountLegalEntityPublicHashingService> _hashingService;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<IMediator> _mediator;
        private OrganisationOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new List<AccountSpecificLegalEntity>()
                });

            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _hashingService = new Mock<IAccountLegalEntityPublicHashingService>();

            _expected = new GetCharityQueryResponse
            {
                Charity = new Charity
                {
                    RegistrationNumber = 12345,
                    Name = "Test Charity",
                    Address1 = "1 Test Street",
                    Address2 = "Test City",
                    Address3 = "Test County",
                    PostCode = "T11 1TT"
                }
            };
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCharityQueryRequest>()))
                .ReturnsAsync(_expected);


            _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object,
                _cookieService.Object, _hashingService.Object);
        }
    }
}