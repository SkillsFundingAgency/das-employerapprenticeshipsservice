﻿using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Orchestrators;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLegalEntitiesControllerTests
{
    public abstract class AccountLegalEntitiesControllerTests
    {
        protected AccountLegalEntitiesController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger> Logger;
        protected Mock<UrlHelper> UrlHelper;
        protected Mock<IMapper> Mapper;
        protected Mock<IHashingService> HashingService;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger>();
            Mapper = new Mock<IMapper>();
            HashingService = new Mock<IHashingService>();
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper.Object, HashingService.Object);
            Controller = new AccountLegalEntitiesController(orchestrator);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;
        }
    }
}
