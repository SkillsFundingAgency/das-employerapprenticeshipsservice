using System.Collections.Generic;
using System.Web.Http.Routing;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger> Logger;
        protected Mock<UrlHelper> UrlHelper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger>();
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object);
            Controller = new EmployerAccountsController(orchestrator);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;

            var accountsResponse = new GetPagedEmployerAccountsResponse { Accounts = new List<Domain.Entities.Account.Account>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>())).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);
        }
    }
}
