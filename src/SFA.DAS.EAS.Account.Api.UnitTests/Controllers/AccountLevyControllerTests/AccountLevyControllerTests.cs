using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.Encoding;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    public abstract class AccountLevyControllerTests
    {
        protected AccountLevyController Controller;        
        protected Mock<ILogger<AccountsOrchestrator>> Logger;
        protected IMapper Mapper;
        protected Mock<IEncodingService> EncodingService;
        protected Mock<IEmployerAccountsApiService> ApiService;
        protected Mock<IEmployerFinanceApiService> FinanceApiService;

        [SetUp]
        public void Arrange()
        {   

            Logger = new Mock<ILogger<AccountsOrchestrator>>();
            EncodingService = new Mock<IEncodingService>();
            ApiService = new Mock<IEmployerAccountsApiService>();
            FinanceApiService = new Mock<IEmployerFinanceApiService>();
            Mapper = ConfigureMapper();
            var orchestrator = new AccountsOrchestrator(Logger.Object, Mapper, EncodingService.Object, ApiService.Object, FinanceApiService.Object);
            Controller = new AccountLevyController(orchestrator);
        }

        private IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load($"SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t))
                .ToList();

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}

