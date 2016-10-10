using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    class WhenICreateAnAgreement
    {
        private EmployerAgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object);
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
                Ref = "12345",
                Text = "Test template",
                CreatedDate = DateTime.Now.AddDays(-20),
                ReleasedDate = DateTime.Now.AddDays(-2)
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
            Assert.AreEqual(entityAddress, response.Data.EmployerAgreement.LegalEntityRegisteredAddress);
            Assert.AreEqual(latestTemplate.Text, response.Data.EmployerAgreement.TemplateText);
            Assert.AreEqual(latestTemplate.Ref, response.Data.EmployerAgreement.TemplateRef);
            Assert.AreEqual(EmployerAgreementStatus.Pending, response.Data.EmployerAgreement.Status);
            Assert.AreEqual(entityRef, response.Data.EmployerAgreement.LegalEntityCode);
            Assert.AreEqual(incorporatedDate, response.Data.EmployerAgreement.LegalEntityIncorporatedDate);
        }
    }
}
