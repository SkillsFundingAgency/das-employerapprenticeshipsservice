using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.InvitationOrchestratorTests;

public class WhenGettingAllInvitations
{
    private Mock<IMediator> _mediator;
    private Mock<ILogger<InvitationOrchestrator>> _logger;
    private InvitationOrchestrator _invitationOrchestrator;

    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
        _mediator.Setup(x => x.Send(It.IsAny<GetUserInvitationsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetUserInvitationsResponse {Invitations = new List<InvitationView> {new InvitationView()} });
        _mediator.Setup(x => x.Send(It.IsAny<GetUserAccountsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetUserAccountsQueryResponse
        {
            Accounts = new Accounts<Account> { AccountList = new List<Account>()}
        });

        _logger = new Mock<ILogger<InvitationOrchestrator>>();

        _invitationOrchestrator = new InvitationOrchestrator(_mediator.Object, _logger.Object, Mock.Of<IEncodingService>());
    }

    [Test]
    public async Task ThenTheMediatorIsCalledToRetrieveTheInvitations()
    {
        //Arrange
        var userId = "123abc";

        //Act
        var actual = await _invitationOrchestrator.GetAllInvitationsForUser(userId);

        //Assert
        _mediator.Verify(x=>x.Send(It.Is<GetUserInvitationsRequest>(c=>c.UserId.Equals(userId)), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsAssignableFrom<UserInvitationsViewModel>(actual.Data);
    }

    [TestCase(1, "Today")]
    [TestCase(2, "1 day")]
    [TestCase(4, "3 days")]
    public async Task ThenTheExpiresInDayIsCorrectlyCalculated(int days, string expectedValue)
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<GetUserInvitationsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetUserInvitationsResponse
        {
            Invitations = new List<InvitationView> {new InvitationView
            {
                ExpiryDate = DateTime.UtcNow.AddDays(days).Date
            } }
        });

        //Act
        var actual = await _invitationOrchestrator.GetAllInvitationsForUser("123abc");

        //Assert
        Assert.AreEqual(expectedValue, actual.Data.Invitations.FirstOrDefault().ExpiryDays());
    }

    [Test]
    public async Task ThenNullIsReturnedWhenAnInvalidRequestExceptionIsThrown()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<GetUserInvitationsRequest>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

        //act
        var actual = await _invitationOrchestrator.GetAllInvitationsForUser("test");

        //Assert
        Assert.IsNull(actual.Data);
    }

    [Test]
    public async Task ThenICheckToSeeWhatAccountsIHaveAccessTo()
    {
        //Arrange
        var expectedUserId = "123FVD";

        //Act
        await _invitationOrchestrator.GetAllInvitationsForUser(expectedUserId);

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetUserAccountsQuery>(c => c.UserRef.Equals(expectedUserId)), It.IsAny<CancellationToken>()), Times.Once);
    }
}