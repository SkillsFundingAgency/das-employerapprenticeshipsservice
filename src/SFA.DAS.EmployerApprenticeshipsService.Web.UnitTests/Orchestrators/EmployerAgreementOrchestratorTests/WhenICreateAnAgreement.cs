using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    class WhenICreateAnAgreement
    {
        private EmployerAgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration(); 

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, _configuration);
        }

        [Test]
        public async Task ThenIShouldGetAnAgreementWithTheLatestTemplate()
        {
            //Assign
            const string hashedId = "2";
            const string userId = "user";
            const string entityName = "Test Corp";
            const string entityRef = "1234ABC";
            const string entityAddress = "Test Street";
            var incorporatedDate = DateTime.Now.AddYears(-10);

            var latestTemplate = new EmployerAgreementTemplate
            {
                Id = 1,
                PartialViewName = "12345",
                CreatedDate = DateTime.Now.AddDays(-20)
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetLatestEmployerAgreementTemplateRequest>()))
                .ReturnsAsync(new GetLatestEmployerAgreementResponse
                {
                    Template = latestTemplate
                });
            
            //Act
            var response = await _orchestrator.Create(hashedId, userId, entityName, entityRef, entityAddress, incorporatedDate);

            //Assert
            Assert.AreEqual(entityName, response.Data.EmployerAgreement.LegalEntityName);
            Assert.AreEqual(entityAddress, response.Data.EmployerAgreement.LegalEntityAddress);
            Assert.AreEqual(latestTemplate.PartialViewName, response.Data.EmployerAgreement.TemplatePartialViewName);
            Assert.AreEqual(EmployerAgreementStatus.Pending, response.Data.EmployerAgreement.Status);
            Assert.AreEqual(entityRef, response.Data.EmployerAgreement.LegalEntityCode);
            Assert.AreEqual(incorporatedDate, response.Data.EmployerAgreement.LegalEntityInceptionDate);
        }
    }
}
