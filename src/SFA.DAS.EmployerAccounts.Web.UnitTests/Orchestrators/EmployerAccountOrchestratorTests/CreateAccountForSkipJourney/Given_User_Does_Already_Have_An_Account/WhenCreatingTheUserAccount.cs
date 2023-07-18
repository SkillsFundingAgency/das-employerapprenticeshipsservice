using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests.CreateAccountForSkipJourney.Given_User_Does_Already_Have_An_Account;
public class WhenCreatingTheUserAccount
{
    private EmployerAccountOrchestrator _employerAccountOrchestrator;
    private Mock<IMediator> _mediator;
    private Mock<ILogger<EmployerAccountOrchestrator>> _logger;
    private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
    private EmployerAccountsConfiguration _configuration;
    private string _existingAccountHashedId = "B3AE4BBE-7E75-4FF3-89B8-3C24FDFCFE10";

    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<EmployerAccountOrchestrator>>();
        _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
        _configuration = new EmployerAccountsConfiguration();

        _employerAccountOrchestrator = new EmployerAccountOrchestrator(
            _mediator.Object, 
            _logger.Object,
            _cookieService.Object, 
            _configuration,
            Mock.Of<IEncodingService>());


        _mediator.Setup(x => x.Send(It.IsAny<GetUserAccountsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserAccountsQueryResponse()
            {
                Accounts = new Accounts<Account> { AccountsCount = 1, AccountList = new List<Account>{ new Account { HashedId = _existingAccountHashedId } } }
            });
    }

    [Test]
    public async Task ThenNewAccountIsNotCreated()
    {
        await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(
            ArrangeModel(),
            It.IsAny<HttpContext>());

        _mediator.Verify(
            x => x.Send<CreateUserAccountCommandResponse>(
                It.IsAny<CreateUserAccountCommand>(), It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Test]
    public async Task ThenResponseContainsHashedIdOfExistingAccount()
    {
        var response =
            await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(new CreateUserAccountViewModel(),
                It.IsAny<HttpContext>());

        Assert.AreEqual(_existingAccountHashedId, response.Data?.HashedId);
    }

    private static CreateUserAccountViewModel ArrangeModel()
    {
        return new CreateUserAccountViewModel
        {
            OrganisationName = "test",
            UserId = Guid.NewGuid().ToString()
        };
    }
}

public class TestUser : User
{
    public TestUser(ICollection<Membership> injectedMemberships)
    {
        Memberships = injectedMemberships;
    }
}