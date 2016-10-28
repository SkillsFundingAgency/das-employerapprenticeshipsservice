using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    class WhenICreateALegalEntity
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
        public async Task ThenTheLegalEntityShouldBeCreated()
        {
            //Assign
            var request = new CreateNewLegalEntity
            {
                HashedId = "1",
                Name = "Test Corp",
                Code = "SD665734",
                Address = "1, Test Street",
                IncorporatedDate = DateTime.Now.AddYears(-20),
                ExternalUserId = "2",
                SignedAgreement = true,
                UserIsAuthorisedToSign = true
            };

            const long legalEntityId = 5;
            const long agreementEntityId = 6;

            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateLegalEntityCommand>()))
                     .ReturnsAsync(new CreateLegalEntityCommandResponse
                     {
                         AgreementView = new EmployerAgreementView
                         {
                             Id = agreementEntityId,
                             LegalEntityId = legalEntityId,
                             LegalEntityName = request.Name,
                             LegalEntityCode = request.Code,
                             LegalEntityRegisteredAddress = request.Address,
                             Status = EmployerAgreementStatus.Pending
                         }
                     });

            //Act
            await _orchestrator.CreateLegalEntity(request);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateLegalEntityCommand>(command =>
            command.LegalEntity.Name.Equals(request.Name) &&
            command.LegalEntity.Code.Equals(request.Code) &&
            command.LegalEntity.RegisteredAddress.Equals(request.Address) &&
            command.LegalEntity.DateOfIncorporation.Equals(request.IncorporatedDate)&&
            command.SignAgreement.Equals(request.SignedAgreement))));
        }
    }
}
