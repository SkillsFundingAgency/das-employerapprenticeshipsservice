using System.Collections.Generic;
using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerFinance.Services;

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
        protected Mock<IEmployerAccountsApiService> ApiService;
        protected Mock<IEmployerFinanceApiService> FinanceApiService;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILog>();
            Mapper = new Mock<IMapper>();
            HashingService = new Mock<IHashingService>();
            ApiService = new Mock<IEmployerAccountsApiService>();
            FinanceApiService = new Mock<IEmployerFinanceApiService>();
          
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, HashingService.Object, ApiService.Object, FinanceApiService.Object);
            Controller = new EmployerAccountsController(orchestrator, ApiService.Object);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;

            var balancesResponse = new GetAccountBalancesResponse { Accounts = new List<AccountBalance>() };
            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(balancesResponse);
        }
    }
}
