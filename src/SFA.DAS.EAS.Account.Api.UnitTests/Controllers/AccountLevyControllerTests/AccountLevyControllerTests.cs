using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    public abstract class AccountLevyControllerTests
    {
        protected AccountLevyController? Controller;
        private Mock<ILogger<AccountsOrchestrator>>? _logger;
        private IMapper? _mapper;
        private Mock<IEncodingService>? _encodingService;
        private Mock<IEmployerAccountsApiService>? _apiService;
        protected Mock<IEmployerFinanceApiService>? FinanceApiService;

        [SetUp]
        public void Arrange()
        {   
            _logger = new Mock<ILogger<AccountsOrchestrator>>();
            _encodingService = new Mock<IEncodingService>();
            _apiService = new Mock<IEmployerAccountsApiService>();
            FinanceApiService = new Mock<IEmployerFinanceApiService>();
            _mapper = ConfigureMapper();
            var orchestrator = new AccountsOrchestrator(_logger.Object, _mapper, _encodingService.Object, _apiService.Object, FinanceApiService.Object);
            Controller = new AccountLevyController(orchestrator);
        }

        private static IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load($"SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t)!)
                .ToList();

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}

