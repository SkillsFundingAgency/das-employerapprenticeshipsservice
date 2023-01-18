using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIGetThePdfAgreement
    {
        private Mock<IMediator> _mediator;
        private Mock<IReferenceDataService> _referenceDataService;
        private EmployerAgreementOrchestrator _orchestrator;
        
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEmployerAgreementPdfResponse { FileStream = new MemoryStream() });
            _mediator.Setup(x => x.Send(It.IsAny<GetSignedEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetSignedEmployerAgreementPdfResponse { FileStream = new MemoryStream() });

            _referenceDataService = new Mock<IReferenceDataService>();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, Mock.Of<IMapper>(), _referenceDataService.Object);
        }

        [Test]
        public async Task ThenWhenIGetTheAgreementTheMediatorIsCalledWithTheCorrectParameters()
        {
            //Act
            await _orchestrator.GetPdfEmployerAgreement("ACC456", "AGB123", "User1");

            //Assert
            _mediator.Verify(
                x => x.Send(It.Is<GetEmployerAgreementPdfRequest>(c => c.HashedAccountId.Equals("ACC456") && c.UserId.Equals("User1") && c.HashedLegalAgreementId.Equals("AGB123")), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenWhenIGetTheSignedAgreementTheMediatorIsCalledWithTheCorrectParameters()
        {
            //Arrange
            var expectedHashedAccountId = "123RDF";
            var expectedHashedAgreementId = "567TGB";
            var expectedUserId = "123AVC";

            //Act
            await
                _orchestrator.GetSignedPdfEmployerAgreement(expectedHashedAccountId, expectedHashedAgreementId,
                    expectedUserId);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<GetSignedEmployerAgreementPdfRequest>(c =>
                c.HashedAccountId.Equals(expectedHashedAccountId) && c.UserId.Equals(expectedUserId) &&
                c.HashedLegalAgreementId.Equals(expectedHashedAgreementId)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheFlashMessageViewModelIsPopulatedWithErrorsWhenAnExceptionOccursAndTheStatusIsSetToBadRequest()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetSignedEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "" } }));

            //Act
            var actual = await _orchestrator.GetSignedPdfEmployerAgreement("", "", "");

            //Assert
            Assert.IsNotEmpty((IEnumerable) actual.FlashMessage.ErrorMessages);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenTheStatusIsSetToUnauhtorizedWhenAnUnauthorizedAccessExceptionIsThrownGettingASignedAgreement()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetSignedEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetSignedPdfEmployerAgreement("", "", "");

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }


        [Test]
        public async Task ThenTheStatusIsSetToUnauhtorizedWhenAnUnauthorizedAccessExceptionIsThrownGettingAnAgreement()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.GetPdfEmployerAgreement("", "", "");

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }
    }
}
