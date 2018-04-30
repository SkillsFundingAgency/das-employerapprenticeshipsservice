using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetThePdfAgreement
    {
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private EmployerAgreementOrchestrator _orchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;

        private readonly Guid _externalUserId = Guid.NewGuid();

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAgreementPdfRequest>()))
                .ReturnsAsync(new GetEmployerAgreementPdfResponse { FileStream = new MemoryStream() });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetSignedEmployerAgreementPdfRequest>()))
                .ReturnsAsync(new GetSignedEmployerAgreementPdfResponse { FileStream = new MemoryStream() });

            _logger = new Mock<ILog>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, _logger.Object, _configuration);
        }

        [Test]
        public async Task ThenWhenIGetTheAgreementTheMediatorIsCalledWithTheCorrectParameters()
        {
            //Act
            await _orchestrator.GetPdfEmployerAgreement("ACC456","AGB123",_externalUserId);

            //Assert
            _mediator.Verify(
                x => x.SendAsync(It.Is<GetEmployerAgreementPdfRequest>(c => c.HashedAccountId.Equals("ACC456") 
                                                                            && c.ExternalUserId.Equals(_externalUserId) 
                                                                            && c.HashedLegalAgreementId.Equals("AGB123"))));
        }

        [Test]
        public async Task ThenWhenIGetTheSignedAgreementTheMediatorIsCalledWithTheCorrectParameters()
        {
            //Arrange
            var expectedHashedAccountId = "123RDF";
            var expectedHashedAgreementId = "567TGB";
            var expectedUserId = Guid.NewGuid();

            //Act
            await
                _orchestrator.GetSignedPdfEmployerAgreement(expectedHashedAccountId, expectedHashedAgreementId,
                    expectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetSignedEmployerAgreementPdfRequest>(c =>
                c.HashedAccountId.Equals(expectedHashedAccountId) && c.ExternalUserId.Equals(expectedUserId) &&
                c.HashedLegalAgreementId.Equals(expectedHashedAgreementId))));
        }

        [Test]
        public async Task ThenTheFlashMessageViewModelIsPopulatedWithErrorsWhenAnExceptionOccursAndTheStatusIsSetToBadRequest()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetSignedEmployerAgreementPdfRequest>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "" } }));

            //Act
            var actual = await _orchestrator.GetSignedPdfEmployerAgreement("", "", _externalUserId);

            //Assert
            Assert.IsNotEmpty(actual.FlashMessage.ErrorMessages);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenTheStatusIsSetToUnauhtorizedWhenAnUnauthorizedAccessExceptionIsThrownGettingASignedAgreement()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetSignedEmployerAgreementPdfRequest>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetSignedPdfEmployerAgreement("", "", _externalUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }


        [Test]
        public async Task ThenTheStatusIsSetToUnauhtorizedWhenAnUnauthorizedAccessExceptionIsThrownGettingAnAgreement()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAgreementPdfRequest>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetPdfEmployerAgreement("", "", _externalUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }
    }
}
