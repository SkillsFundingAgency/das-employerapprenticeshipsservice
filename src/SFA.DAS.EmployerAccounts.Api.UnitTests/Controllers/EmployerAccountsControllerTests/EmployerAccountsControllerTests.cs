using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger<AccountsOrchestrator>> Logger;
        protected Mock<IEncodingService> EncodingService;
        protected Mock<IUrlHelper> UrlTestHelper;
        protected Mock<IMapper> Mapper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger<AccountsOrchestrator>>();
            EncodingService = new Mock<IEncodingService>();
            Mapper = new Mock<IMapper>();

            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, EncodingService.Object);
            Controller = new EmployerAccountsController(orchestrator, EncodingService.Object);

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