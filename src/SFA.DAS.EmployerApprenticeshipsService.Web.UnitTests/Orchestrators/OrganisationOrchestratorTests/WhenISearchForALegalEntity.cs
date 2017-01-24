using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenISearchForALegalEntity
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
                .ReturnsAsync(new GetAccountLegalEntitiesResponse {Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()} });

            _cookieService = new Mock<ICookieService>();

            _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _cookieService.Object);
        }

        [Test]
        public async Task ThenTheCompanyDetailsAreMappedToTheModel()
        {
            //Arrange
            var expected = new GetEmployerInformationResponse
            {
                CompanyName = "Test Corp",
                CompanyNumber = "0123456",
                DateOfIncorporation = DateTime.Now,
                AddressLine1 = "1 Test Road",
                AddressLine2 = "Test Park",
                TownOrCity = "Test City",
                County = "Testshire",
                AddressPostcode = "TE12 3ST",
                CompanyStatus = "active"
            }; 
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerInformationRequest>()))
                .ReturnsAsync(expected);
            
            //Act
            var actual = await _orchestrator.GetLimitedCompanyByRegistrationNumber(string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.CompanyStatus,actual.Data.Status);
            Assert.AreEqual(expected.CompanyName,actual.Data.Name);
            Assert.AreEqual(expected.DateOfIncorporation,actual.Data.DateOfInception);
            Assert.AreEqual(expected.CompanyNumber,actual.Data.ReferenceNumber);
            Assert.AreEqual($"{expected.AddressLine1}, {expected.AddressLine2}, {expected.TownOrCity}, {expected.County}, {expected.AddressPostcode}", actual.Data.Address);
        }


        [Test]
        public async Task ThenTheCompanyAddressOnlyIncludedPopulatedValues()
        {
            //Arrange
            var expected = new GetEmployerInformationResponse
            {
                CompanyName = "Test Corp",
                CompanyNumber = "0123456",
                DateOfIncorporation = DateTime.Now,
                AddressLine1 = "1 Test Road",
                AddressPostcode = "TE12 3ST",
                CompanyStatus = "active"
            };
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerInformationRequest>()))
                .ReturnsAsync(expected);

            //Act
            var actual = await _orchestrator.GetLimitedCompanyByRegistrationNumber(string.Empty, string.Empty, string.Empty);

            //Assert
          
            Assert.AreEqual($"{expected.AddressLine1}, {expected.AddressPostcode}", actual.Data.Address);
        }

        [Test]
        public async Task ThenAnyPublicOrganisationsShouldBeMarkedAsAlreadyAddedIfTheyHaveBeen()
        {
            //Arrange
            const string addedEntityName = "Added entity";
            const string notAddedEntityName = "Not added entity";

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new LegalEntities
                    {
                        LegalEntityList = new List<LegalEntity>
                        {
                            new LegalEntity
                            {
                                Name = addedEntityName
                            }
                        }
                    }
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPublicSectorOrganisationQuery>()))
                .ReturnsAsync(new GetPublicSectorOrganisationResponse
                {
                    Organisaions = new PagedResponse<PublicSectorOrganisation>
                    {
                        Data = new List<PublicSectorOrganisation>
                        {
                            new PublicSectorOrganisation {Name = addedEntityName},
                            new PublicSectorOrganisation {Name = notAddedEntityName }
                        }
                    }
                });
                

            //Act
            var actual = await _orchestrator.FindPublicSectorOrganisation("test", "362546752", string.Empty);
            
            //Assert
            Assert.IsNotNull(actual?.Data?.Results?.Data);
            Assert.AreEqual(2, actual.Data.Results.Data.Count);
            Assert.IsTrue(actual.Data.Results.Data.Single(x => x.Name.Equals(addedEntityName)).AddedToAccount);
            Assert.IsFalse(actual.Data.Results.Data.Single(x => x.Name.Equals(notAddedEntityName)).AddedToAccount);
        }

        [Test]
        public async Task ThenPublicSectorBodiesShouldBeMarkedAsSuch()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse
                {
                    Entites = new LegalEntities()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPublicSectorOrganisationQuery>()))
                .ReturnsAsync(new GetPublicSectorOrganisationResponse
                {
                    Organisaions = new PagedResponse<PublicSectorOrganisation>
                    {
                        Data = new List<PublicSectorOrganisation>
                        {
                            new PublicSectorOrganisation {Name = "Test Org"},
                        }
                    }
                });


            //Act
            var actual = await _orchestrator.FindPublicSectorOrganisation("test", string.Empty, string.Empty);

            //Assert
            Assert.IsNotNull(actual?.Data?.Results?.Data);
            Assert.AreEqual(OrganisationType.PublicBodies, actual.Data.Results.Data.First().Type);
           
        }
    }
}
