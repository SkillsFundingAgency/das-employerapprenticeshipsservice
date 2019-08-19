using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    public abstract class AccountPayeSchemesControllerTests
    {
        protected AccountPayeSchemesController Controller;
        protected EmployerAccountsApiConfiguration Configuration;
        protected Mock<IMediator> Mediator;
        protected Mock<ILog> Logger;
        protected Mock<UrlHelper> UrlHelper;
        protected Mock<IMapper> Mapper;
        protected Mock<IHashingService> HashingService;
        protected Mock<IEmployerAccountsApiService> ApiService;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILog>();
            Mapper = new Mock<IMapper>();
            HashingService = new Mock<IHashingService>();
            Configuration = new EmployerAccountsApiConfiguration();
            ApiService = new Mock<IEmployerAccountsApiService>();

            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, HashingService.Object, ApiService.Object);
            Controller = new AccountPayeSchemesController(orchestrator, Configuration);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;
        }
    }
}
