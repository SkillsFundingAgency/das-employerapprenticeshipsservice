using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests;

public class WhenGettingAccount
{
    private const string HashedAccountId = "ABC123";
    private const long AccountId = 123;
    private const string UserId = "USER1";

    private Mock<IMediator> _mediator;
    private Mock<IEncodingService> _encodingServiceMock;
    private EmployerTeamOrchestrator _orchestrator;        
    private AccountStats _accountStats;
    private Mock<ICurrentDateTime> _currentDateTime;
    private Mock<IAccountApiClient> _accountApiClient;
    private Mock<IMapper> _mapper;
    private List<AccountTask> _tasks;
    private AccountTask _testTask;
    private DateTime _lastTermsAndConditionsUpdate;

    [SetUp]
    public void Arrange()
    {
        _accountStats = new AccountStats
        {
            AccountId = 10,
            OrganisationCount = 3,
            PayeSchemeCount = 4,
            TeamMemberCount = 8
        };

        _testTask = new AccountTask
        {
            Type = "Test",
            ItemsDueCount = 2
        };

        _tasks = new List<AccountTask>
        {
            _testTask
        };

        _mediator = new Mock<IMediator>();
        _mediator.Setup(m => m.Send(It.Is<GetEmployerAccountByIdQuery>(q => q.AccountId == AccountId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetEmployerAccountByIdResponse
            {
                Account = new Account
                {
                    HashedId = HashedAccountId,
                    Id = AccountId,
                    Name = "Account 1"
                }
            });

        _mediator.Setup(x => x.Send(It.IsAny<GetAccountTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAccountTasksResponse
            {
                Tasks = _tasks
            });

        _mediator.Setup(m => m.Send(It.Is<GetUserAccountRoleQuery>(q => q.ExternalUserId == UserId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserAccountRoleResponse
            {
                UserRole = Role.Owner
            });

        _mediator.Setup(m => m.Send(It.IsAny<GetUserByRefQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserByRefResponse
            {
                User =  new SFA.DAS.EmployerAccounts.Models.UserProfile.User
                {
                    TermAndConditionsAcceptedOn = DateTime.Now
                }
            });

        _mediator.Setup(m => m.Send(It.Is<GetAccountEmployerAgreementsRequest>(q => q.AccountId == AccountId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = new List<EmployerAgreementStatusDto>
                {
                    new EmployerAgreementStatusDto
                    {
                        Pending = new EmployerAgreementDetailsDto { Id = 123 }
                    },
                    new EmployerAgreementStatusDto
                    {
                        Pending = new EmployerAgreementDetailsDto { Id = 124 }
                    },
                    new EmployerAgreementStatusDto
                    {
                        Pending = new EmployerAgreementDetailsDto { Id = 125 }
                    },
                    new EmployerAgreementStatusDto
                    {
                        Pending = new EmployerAgreementDetailsDto { Id = 126 }
                    },
                    new EmployerAgreementStatusDto
                    {
                        Signed = new SignedEmployerAgreementDetailsDto { Id = 111 }
                    },
                    new EmployerAgreementStatusDto
                    {
                        Signed = new SignedEmployerAgreementDetailsDto { Id = 112 }
                    },
                    new EmployerAgreementStatusDto
                    {
                        Signed = new SignedEmployerAgreementDetailsDto { Id = 113 }
                    }
                }
            });

        _mediator.Setup(x => x.Send(It.IsAny<GetTeamMemberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTeamMemberResponse{User = new MembershipView{FirstName = "Bob"}});

        _mediator.Setup(x => x.Send(It.IsAny<GetAccountStatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAccountStatsResponse {Stats = _accountStats});

        _currentDateTime = new Mock<ICurrentDateTime>();

        _accountApiClient = new Mock<IAccountApiClient>();

        _accountApiClient.Setup(c => c.GetAccount(HashedAccountId)).ReturnsAsync(new AccountDetailViewModel
            {ApprenticeshipEmployerType = "Levy"});
           
        _mapper = new Mock<IMapper>();

        _lastTermsAndConditionsUpdate = DateTime.Now.AddDays(-10);
        var employerAccountsConfiguration = new EmployerAccountsConfiguration { LastTermsAndConditionsUpdate = _lastTermsAndConditionsUpdate };

        _encodingServiceMock = new Mock<IEncodingService>();

        _encodingServiceMock.Setup(e => e.Decode(HashedAccountId, EncodingType.AccountId)).Returns(AccountId);

        _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, _currentDateTime.Object, _accountApiClient.Object, _mapper.Object, employerAccountsConfiguration, _encodingServiceMock.Object);
    }
        
    [Test]
    public async Task ThenShouldGetAccountStats()
    {
        // Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.IsNotNull(actual.Data);
        Assert.AreEqual(_accountStats.OrganisationCount, actual.Data.OrganisationCount);
        Assert.AreEqual(_accountStats.PayeSchemeCount, actual.Data.PayeSchemeCount);
        Assert.AreEqual(_accountStats.TeamMemberCount, actual.Data.TeamMemberCount);
    }

    [Test]
    public async Task ThenShouldDisplayTermsAndConditionBanner()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetUserByRefQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserByRefResponse
            {
                User = new SFA.DAS.EmployerAccounts.Models.UserProfile.User
                {
                    TermAndConditionsAcceptedOn = _lastTermsAndConditionsUpdate.AddDays(-1)
                }
            });

        // Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.IsNotNull(actual.Data);
        Assert.AreEqual(true, actual.Data.ShowTermsAndConditionBanner);
    }

    [Test]
    public async Task ThenShouldNotDisplayTermsAndConditionBanner()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetUserByRefQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserByRefResponse
            {
                User = new SFA.DAS.EmployerAccounts.Models.UserProfile.User
                {
                    TermAndConditionsAcceptedOn = _lastTermsAndConditionsUpdate.AddDays(1)
                }
            });

        // Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.IsNotNull(actual.Data);
        Assert.AreEqual(false, actual.Data.ShowTermsAndConditionBanner);
    }

    [Test]
    public async Task ThenShouldReturnTasks()
    {
        // Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        _mediator.Verify(m => m.Send(It.IsAny<GetAccountTasksQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNotNull(actual.Data);
        Assert.Contains(_testTask, actual.Data.Tasks.ToArray());
    }

    [Test]
    public async Task ThenIShouldNotReturnTasksWithZeroItems()
    {
        //Arrange
        _testTask.ItemsDueCount = 0;

        // Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.IsNotNull(actual.Data);
        Assert.IsEmpty(actual.Data.Tasks);
    }

    [Test]
    public async Task ThenShouldReturnNoTasksIfANullIsReturnedFromTaskQuery()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<GetAccountTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.IsNotNull(actual.Data);
        Assert.IsEmpty(actual.Data.Tasks);
    }

    [Test]
    public async Task ThenShouldReturnAccountsTasks()
    {
        //Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.AreEqual(_tasks, actual.Data.Tasks);
        _mediator.Verify(x => x.Send(It.Is<GetAccountTasksQuery>(r => r.AccountId.Equals(AccountId)), It.IsAny<CancellationToken>()),Times.Once);
    }

    [Test]
    public async Task ThenShouldSetAccountHashId()
    {
        //Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.AreEqual(HashedAccountId, actual.Data.HashedAccountId);
    }

    [TestCase("2017-10-19", true, Description = "Banner visible")]
    [TestCase("2017-10-19 11:59:59", true, Description = "Banner visible until midnight")]
    [TestCase("2017-10-20 00:00:00", false, Description = "Banner hidden after midnight")]
    public async Task ThenDisplayOfAcademicYearBannerIsDetermined(DateTime now, bool expectShowBanner)
    {
        //Arrange
        _currentDateTime.Setup(x => x.Now).Returns(now);

        //Act
        var model = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.AreEqual(expectShowBanner, model.Data.ShowAcademicYearBanner);
    }

    [Test]
    public async Task ThenAgreementsAreRetrievedCorrectly()
    {
        //Act
        var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.AreEqual(3, actual.Data.SignedAgreementCount);
        Assert.AreEqual(4, actual.Data.RequiresAgreementSigning);
    }

    [TestCase(Common.Domain.Types.ApprenticeshipEmployerType.Levy, "Levy")]
    [TestCase(Common.Domain.Types.ApprenticeshipEmployerType.NonLevy, "NonLevy")]
    public async Task ThenShouldReturnCorrectApprenticeshipEmployerTypeFromAccountApi(Common.Domain.Types.ApprenticeshipEmployerType expectedApprenticeshipEmployerType, string apiApprenticeshipEmployerType)
    {
        //Arrange
        _accountApiClient
            .Setup(c => c.GetAccount(HashedAccountId))
            .ReturnsAsync(new AccountDetailViewModel
                { ApprenticeshipEmployerType = apiApprenticeshipEmployerType });

        //Act
        var model = await _orchestrator.GetAccount(HashedAccountId, UserId);

        //Assert
        Assert.AreEqual(expectedApprenticeshipEmployerType, model.Data.ApprenticeshipEmployerType);
    }
        
    [Test]
    public async Task ThenReturnAccountSummary()
    {
        var model = await _orchestrator.GetAccountSummary(HashedAccountId, UserId);
        Assert.IsNotNull(model.Data);
    }
}