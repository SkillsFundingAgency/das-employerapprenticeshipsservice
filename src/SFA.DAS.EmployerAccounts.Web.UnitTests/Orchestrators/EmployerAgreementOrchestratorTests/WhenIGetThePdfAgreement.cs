using System.IO;
using AutoFixture.NUnit3;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests;

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

        _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, Mock.Of<IMapper>(), _referenceDataService.Object, Mock.Of<IEncodingService>());
    }

    [Test, MoqAutoData]
    public async Task ThenWhenIGetTheAgreementTheMediatorIsCalledWithTheCorrectParameters(
        string hashedAccountId,
        string hashedAgreementId,
        string userId,
        long accountId,
        long agreementId,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        EmployerAgreementOrchestrator orchestrator)
    {
        // Arrange
        encodingServiceMock.Setup(e => e.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        encodingServiceMock.Setup(e => e.Decode(hashedAgreementId, EncodingType.AccountId)).Returns(agreementId);

        //Act
        await orchestrator.GetPdfEmployerAgreement(hashedAccountId, hashedAgreementId, userId);

        //Assert
        mediatorMock.Verify(x => x.Send(It.Is<GetEmployerAgreementPdfRequest>(c => c.AccountId == accountId && c.UserId == userId && c.LegalAgreementId == agreementId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ThenWhenIGetTheSignedAgreementTheMediatorIsCalledWithTheCorrectParameters(
        string hashedAccountId,
        string hashedAgreementId,
        string userId,
        long accountId,
        long agreementId,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        EmployerAgreementOrchestrator orchestrator)
    {
        //Arrange
        encodingServiceMock.Setup(e => e.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
        encodingServiceMock.Setup(e => e.Decode(hashedAgreementId, EncodingType.AccountId)).Returns(agreementId);

        //Act
        await orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId, hashedAgreementId, userId);

        //Assert
        mediatorMock.Verify(x => x.Send(It.Is<GetSignedEmployerAgreementPdfRequest>(c =>
            c.AccountId.Equals(accountId) && c.UserId.Equals(userId) &&
            c.LegalAgreementId.Equals(agreementId)), It.IsAny<CancellationToken>()));
    }

    [Test, MoqAutoData]
    public async Task ThenTheFlashMessageViewModelIsPopulatedWithErrorsWhenAnExceptionOccursAndTheStatusIsSetToBadRequest(
        string hashedAccountId,
        string hashedAgreementId,
        string userId,
        [Frozen] Mock<IMediator> mediatorMock,
        EmployerAgreementOrchestrator orchestrator)
    {
        //Arrange
        mediatorMock.Setup(x => x.Send(It.IsAny<GetSignedEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "" } }));

        //Act
        var actual = await orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId, hashedAgreementId, userId);

        //Assert
        Assert.IsNotEmpty(actual.FlashMessage.ErrorMessages);
        Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
    }

    [Test, MoqAutoData]
    public async Task ThenTheStatusIsSetToUnauhtorizedWhenAnUnauthorizedAccessExceptionIsThrownGettingASignedAgreement(
        string hashedAccountId,
        string hashedAgreementId,
        string userId,
        [Frozen] Mock<IMediator> mediatorMock,
        EmployerAgreementOrchestrator orchestrator)
    {
        //Arrange
        mediatorMock.Setup(x => x.Send(It.IsAny<GetSignedEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        //Act
        var actual = await orchestrator.GetSignedPdfEmployerAgreement(hashedAccountId, hashedAgreementId, userId);

        //Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
    }


    [Test, MoqAutoData]
    public async Task ThenTheStatusIsSetToUnauhtorizedWhenAnUnauthorizedAccessExceptionIsThrownGettingAnAgreement(
        string hashedAccountId,
        string hashedAgreementId,
        string userId,
        [Frozen] Mock<IMediator> mediatorMock,
        EmployerAgreementOrchestrator orchestrator)
    {
        //Arrange
        mediatorMock.Setup(x => x.Send(It.IsAny<GetEmployerAgreementPdfRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        //Act
        var actual = await orchestrator.GetPdfEmployerAgreement(hashedAccountId, hashedAgreementId, userId);

        //Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
    }
}