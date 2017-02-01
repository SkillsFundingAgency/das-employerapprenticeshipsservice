using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenISearchForACharity
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IMapper> _mapper;
        private Mock<ICookieService> _cookieService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _mapper = new Mock<IMapper>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()}
                });

            _cookieService = new Mock<ICookieService>();

            _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object,
                _cookieService.Object);
        }

        [Test]
        public async Task ThenTheCharityDetailsAreMappedToTheModel()
        {
            //Arrange
            var expected = new GetCharityQueryResponse
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
                .ReturnsAsync(expected);

            //Act
            var actual = await _orchestrator.GetCharityByRegistrationNumber(string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Charity.RegistrationNumber.ToString(), actual.Data.ReferenceNumber);
            Assert.AreEqual(expected.Charity.Name, actual.Data.Name);
            Assert.AreEqual(expected.Charity.FormattedAddress, actual.Data.Address);
        }

    }
}
