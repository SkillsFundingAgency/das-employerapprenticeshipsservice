using System.Web.Http.Routing;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    public abstract class AccountPayeSchemesControllerTests
    {
        protected AccountPayeSchemesController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger> Logger;
        protected Mock<UrlHelper> UrlHelper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger>();
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object);
            Controller = new AccountPayeSchemesController(orchestrator);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;
        }
    }
}
