using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenISearchForACharity
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private GetCharityQueryResponse _expected;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()}
                });

            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

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
                _cookieService.Object);
        }

        [Test]
        public async Task ThenTheCharityDetailsAreMappedToTheModel()
        {
           //Act
            var actual = await _orchestrator.GetCharityByRegistrationNumber(string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_expected.Charity.RegistrationNumber.ToString(), actual.Data.ReferenceNumber);
            Assert.AreEqual(_expected.Charity.Name, actual.Data.Name);
            Assert.AreEqual(_expected.Charity.FormattedAddress, actual.Data.Address);
        }

        

        [Test]
        public async Task ThenTheStatusWillBeSetToConflictWhenTheLegalEntityHasAlreadyBeenAddedAndTheCharityHasLeadingAndTrailingWhitespace()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new LegalEntities { LegalEntityList = new List<LegalEntity>
                    {
                        new LegalEntity
                        {
                            Code="12345",
                            Source = (short)OrganisationType.Charities
                        }
                    } }
                });

            //Act
            var actual = await _orchestrator.GetCharityByRegistrationNumber(" 12345 ", "123", string.Empty);

            //Assert
            Assert.AreEqual(HttpStatusCode.Conflict,actual.Status);
        }

    }
}
