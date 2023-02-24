using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests;

public class WhenIRemoveTheLegalAgreement
{
    private Mock<IMediator> _mediator;
    private Mock<IReferenceDataService> _referenceDataService;
    private Mock<IEncodingService> _encodingServiceMock;
    private EmployerAgreementOrchestrator _orchestrator;

    private const long ExpectedAccountId = 456;
    private const long ExpectedAccountLegalEntitytId = 56;
    private const string ExpectedHashedAccountId = "RT456";
    private const string ExpectedHashedAccountLegalEntitytId = "RRTE56";
    private const string ExpectedUserId = "TYG68UY";

    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
            
        _referenceDataService = new Mock<IReferenceDataService>();

        _encodingServiceMock = new Mock<IEncodingService>();

        _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, Mock.Of<IMapper>(), _referenceDataService.Object, _encodingServiceMock.Object);
    }
        
    [Test]
    public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<RemoveLegalEntityCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

        //Act
        var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmOrganisationToRemoveViewModel(), ExpectedUserId);

        //Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
    }

    [Test]
    public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<RemoveLegalEntityCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new UnauthorizedAccessException());

        //Act
        var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmOrganisationToRemoveViewModel { Name = "TestName" }, ExpectedUserId);

        //Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
    }

    [Test]
    public async Task ThenIfTheCommandIsValidTheFlashMessageIsPopulated()
    {
        //Arrange
        _encodingServiceMock.Setup(e => e.Decode(ExpectedHashedAccountId, EncodingType.AccountId)).Returns(ExpectedAccountId);
        _encodingServiceMock.Setup(e => e.Decode(ExpectedHashedAccountLegalEntitytId, EncodingType.PublicAccountLegalEntityId)).Returns(ExpectedAccountLegalEntitytId);

        //Act
        var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmOrganisationToRemoveViewModel { Name = "TestName", HashedAccountId = ExpectedHashedAccountId, HashedAccountLegalEntitytId = ExpectedHashedAccountLegalEntitytId }, ExpectedUserId);

        //Assert
        _mediator.Verify(x => x.Send(It.Is<RemoveLegalEntityCommand>(
            c => c.AccountId.Equals(ExpectedAccountId)
                 && c.AccountLegalEntityId.Equals(ExpectedAccountLegalEntitytId)
                 && c.UserId.Equals(ExpectedUserId)), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNotNull(actual);
        Assert.IsNotNull(actual.FlashMessage);
        Assert.AreEqual("You have removed TestName.", actual.FlashMessage.Headline);

    }
}