using System.Collections.Generic;
using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILog> Logger;
        protected Mock<UrlHelper> UrlHelper;
        protected Mock<IMapper> Mapper;
        protected Mock<IHashingService> HashingService;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILog>();
            Mapper = new Mock<IMapper>();
            HashingService = new Mock<IHashingService>();
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, HashingService.Object);
            Controller = new EmployerAccountsController(orchestrator);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;

            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                Accounts = new List<Domain.Models.Account.Account>()
            };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsQuery>())).ReturnsAsync(accountsResponse);

            var balancesResponse = new GetAccountBalancesResponse {Accounts = new List<AccountBalance>()};
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);
        }
    }

}
