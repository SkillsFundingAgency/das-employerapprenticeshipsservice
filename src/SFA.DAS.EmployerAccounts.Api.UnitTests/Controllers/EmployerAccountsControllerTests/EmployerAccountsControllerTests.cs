using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger<AccountsOrchestrator>> Logger;
        protected Mock<IHashingService> HashingService;
        protected Mock<IUrlHelper> UrlTestHelper;
        protected Mock<IMapper> Mapper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger<AccountsOrchestrator>>();
            HashingService = new Mock<IHashingService>();
            Mapper = new Mock<IMapper>();

            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, HashingService.Object);
            Controller = new EmployerAccountsController(orchestrator);

            UrlTestHelper = new Mock<IUrlHelper>();
            Controller.Url = UrlTestHelper.Object;

            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                Accounts = new List<Account>()
            };

            Mediator.Setup(x => x.Send(It.IsAny<GetPagedEmployerAccountsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(accountsResponse);
        }
    }
}
