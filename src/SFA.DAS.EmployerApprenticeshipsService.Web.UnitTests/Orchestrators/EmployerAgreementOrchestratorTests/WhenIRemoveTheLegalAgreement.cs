using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.RemoveLegalEntity;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIRemoveTheLegalAgreement
    {
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private EmployerAgreementOrchestrator _orchestrator;

        private const string ExpectedHahsedAccountId = "RT456";
        private const string ExpectedHashedAgreementId = "RRTE56";
        private const string ExpectedUserId = "TYG68UY";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            //RemoveLegalEntityCommand

            _logger = new Mock<ILog>();

            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, _configuration);
        }

        [Test]
        public async Task ThenIfTheRemoveOrganisationConfirmCheckHasNotBeenSelectedTheFlashMessageIsPopulatedAndReturned()
        {
            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmLegalAgreementToRemoveViewModel {}, ExpectedUserId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<OrchestratorResponse<bool>>(actual);
            Assert.IsNotNull(actual.FlashMessage);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfTheRemoveOrganisationCheckHasBeenSelectedAsNoThenNoChangesAreMade()
        {
            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmLegalAgreementToRemoveViewModel { RemoveOrganisation = 1}, ExpectedUserId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.Continue, actual.Status);
        }


        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveLegalEntityCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmLegalAgreementToRemoveViewModel { RemoveOrganisation = 2 }, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveLegalEntityCommand>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmLegalAgreementToRemoveViewModel { RemoveOrganisation = 2, Name = "TestName"}, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenIfTheCommandIsValidTheFlashMessageIsPopulated()
        {
            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmLegalAgreementToRemoveViewModel { RemoveOrganisation = 2, Name = "TestName",HashedAccountId = ExpectedHahsedAccountId, HashedAgreementId = ExpectedHashedAgreementId}, ExpectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<RemoveLegalEntityCommand>(
                c=>c.HashedAccountId.Equals(ExpectedHahsedAccountId) 
                && c.HashedLegalAgreementId.Equals(ExpectedHashedAgreementId) 
                && c.UserId.Equals(ExpectedUserId))),Times.Once);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.FlashMessage);
            Assert.AreEqual("You have removed TestName.", actual.FlashMessage.Headline);

        }
    }
}
