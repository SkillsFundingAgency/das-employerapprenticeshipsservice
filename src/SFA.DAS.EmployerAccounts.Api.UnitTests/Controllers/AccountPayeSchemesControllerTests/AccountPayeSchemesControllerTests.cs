using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    public abstract class AccountPayeSchemesControllerTests
    {
        protected AccountPayeSchemesController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILog> Logger;
        protected Mock<IUrlHelper> UrlHelper;
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
            Controller = new AccountPayeSchemesController(orchestrator);

            Microsoft.AspNetCore.Mvc.IUrlHelper = new Mock<IUrlHelper>();
            Controller.Url = Microsoft.AspNetCore.Mvc.Routing.UrlHelper.Object;
        }
    }
}
