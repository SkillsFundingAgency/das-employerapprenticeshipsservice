using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public abstract class EmployerAccountsControllerTests
    {
        protected EmployerAccountsController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILog> Logger;
        protected Mock<IHashingService> HashingService;
        protected Mock<IUrlHelper> UrlHelper;
        protected Mock<IMapper> Mapper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILog>();
            HashingService = new Mock<IHashingService>();
            Mapper = new Mock<IMapper>();

            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, HashingService.Object);
            Controller = new EmployerAccountsController(orchestrator);

            Microsoft.AspNetCore.Mvc.IUrlHelper = new Mock<IUrlHelper>();
            Controller.Url = Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Object;

            var accountsResponse = new GetPagedEmployerAccountsResponse
            {
                Accounts = new List<Account>()
            };

            Mediator.Setup(x => x.Send(It.IsAny<GetPagedEmployerAccountsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(accountsResponse);
        }
    }
}
