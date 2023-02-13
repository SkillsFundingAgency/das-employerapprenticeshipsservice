using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests;

public class WhenRenamingAnAccount
{
    private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
    private Mock<IMediator> _mediator;
    private EmployerAccountOrchestrator _orchestrator;
    private EmployerAccountsConfiguration _configuration;
    private Account _account;

    [SetUp]
    public void Arrange()
    {
        _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
        _mediator = new Mock<IMediator>();
        _configuration = new EmployerAccountsConfiguration();

        _account = new Account
        {
            Id = 123,
            HashedId = "ABC123",
            Name = "Test Account"
        };

        _mediator.Setup(x => x.Send(It.IsAny<GetEmployerAccountByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetEmployerAccountByIdResponse { Account = _account });

        _mediator.Setup(x => x.Send(It.IsAny<GetUserAccountRoleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserAccountRoleResponse { UserRole = Role.Owner });

        _orchestrator = new EmployerAccountOrchestrator(_mediator.Object, Mock.Of<ILogger<EmployerAccountOrchestrator>>(), _cookieService.Object, _configuration, Mock.Of<IEncodingService>());
    }

    [Test]
    public async Task ThenTheCorrectAccountDetailsShouldBeReturned()
    {
        //Act
        var response = await _orchestrator.GetEmployerAccount(1231);

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetEmployerAccountByIdQuery>(q => q.AccountId.Equals(_account.HashedId)), It.IsAny<CancellationToken>()));
        Assert.AreEqual(_account.HashedId, response.Data.HashedId);
        Assert.AreEqual(_account.Name, response.Data.Name);
        Assert.AreEqual(HttpStatusCode.OK, response.Status);
    }

    [Test]
    public async Task ThenTheAccountNameShouldBeUpdated()
    {
        //Act
        var response = await _orchestrator.RenameEmployerAccount(new RenameEmployerAccountViewModel
        {
            NewName = "New Account Name"
        }, "ABC123");

        //Assert
        Assert.IsInstanceOf<OrchestratorResponse<RenameEmployerAccountViewModel>>(response);

        _mediator.Verify(x =>
                x.Send(It.Is<RenameEmployerAccountCommand>(c => c.NewName == "New Account Name"), It.IsAny<CancellationToken>()),
            Times.Once());
    }
}