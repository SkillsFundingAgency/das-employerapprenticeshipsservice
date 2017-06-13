using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Castle.Core.Internal;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Orchestrators;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    public abstract class AccountLevyControllerTests
    {
        protected AccountLevyController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<ILog> Logger;
        protected IMapper Mapper;
        protected Mock<IHashingService> HashingService;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Logger = new Mock<ILog>();
            HashingService = new Mock<IHashingService>();
            Mapper = ConfigureMapper();
            var orchestrator = new AccountsOrchestrator(Mediator.Object, Logger.Object, Mapper, HashingService.Object);
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
