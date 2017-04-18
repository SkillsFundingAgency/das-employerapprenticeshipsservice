using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Castle.Core.Internal;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    public abstract class AccountLevyControllerTests
    {
        protected AccountLevyController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILogger> Logger;
        protected IMapper Mapper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILogger>();
            Mapper = ConfigureMapper();
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper);
            Controller = new AccountLevyController(orchestrator);
        }

        private IMapper ConfigureMapper()
        {
            var apiProfiles = Assembly.Load($"SFA.DAS.EAS.Api").GetTypes()
                            .Where(t => typeof(Profile).IsAssignableFrom(t))
                            .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                apiProfiles.ForEach(cfg.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}
