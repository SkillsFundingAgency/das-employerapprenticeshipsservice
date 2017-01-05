using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenISearchForALegalEntity
    {
        private Web.Orchestrators.OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountLegalEntitiesRequest>()))
                .ReturnsAsync(new GetAccountLegalEntitiesResponse {Entites = new LegalEntities {LegalEntityList = new List<LegalEntity>()} });

            _orchestrator = new Web.Orchestrators.OrganisationOrchestrator(_mediator.Object, _logger.Object);
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
                AddressLine2 = "Test City",
                AddressPostcode = "TE12 3ST",
                CompanyStatus = "active"
            }; ;
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerInformationRequest>()))
                .ReturnsAsync(expected);
            
            //Act
            var actual = await _orchestrator.GetLimitedCompanyByRegistrationNumber("", "", "");

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.CompanyStatus,actual.Data.CompanyStatus);
            Assert.AreEqual(expected.CompanyName,actual.Data.Name);
            Assert.AreEqual(expected.DateOfIncorporation,actual.Data.DateOfInception);
            Assert.AreEqual(expected.CompanyNumber,actual.Data.CompanyNumber);
            Assert.AreEqual($"{expected.AddressLine1}, {expected.AddressLine2}, {expected.AddressPostcode}", actual.Data.Address);
        }
    }
}
