using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Account.Api.ServiceRegistrations;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(AccountsOrchestrator))]
    [TestCase(typeof(AccountTransactionsOrchestrator))]
    [TestCase(typeof(StatisticsOrchestrator))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        RunTestForType(toResolve);
    }
    
    [TestCase(typeof(IEmployerAccountsApiService))]
    [TestCase(typeof(IEmployerFinanceApiService))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_ApiServices(Type toResolve)
    {
        RunTestForType(toResolve);
    }
        
    [TestCase(typeof(AccountLegalEntitiesController))]
    [TestCase(typeof(AccountLevyController))]
    [TestCase(typeof(AccountPayeSchemesController))]
    [TestCase(typeof(AccountTransactionsController))]
    [TestCase(typeof(EmployerAccountsController))]
    [TestCase(typeof(EmployerAgreementController))]
    [TestCase(typeof(EmployerUserController))]
    [TestCase(typeof(LegalEntitiesController))]
    [TestCase(typeof(StatisticsController))]
    [TestCase(typeof(TransferConnectionsController))]
    [TestCase(typeof(UserController))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Controllers(Type toResolve)
    {
        RunTestForType(toResolve);
    }

    private static void RunTestForType(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.That(type, Is.Not.Null);
    }

    private static void SetupServiceCollection(IServiceCollection services)
    {
        var mockHostingEnvironment = new Mock<IWebHostEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var config = GenerateConfiguration();
        services.AddSingleton(mockHostingEnvironment.Object);
        services.AddAutoMapper(typeof(Startup).Assembly);
        services.AddApiConfigurationSections(config);
        
        services.AddClientServices();
        services.AddOrchestrators();

        services.AddSingleton<IEncodingService, EncodingService>();

        services.AddLogging();

        services.AddTransient<AccountLegalEntitiesController>();
        services.AddTransient<AccountLevyController>();
        services.AddTransient<AccountPayeSchemesController>();
        services.AddTransient<AccountTransactionsController>();
        services.AddTransient<EmployerAccountsController>();
        services.AddTransient<EmployerAgreementController>();
        services.AddTransient<EmployerUserController>();
        services.AddTransient<LegalEntitiesController>();
        services.AddTransient<StatisticsController>();
        services.AddTransient<TransferConnectionsController>();
        services.AddTransient<UserController>();
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("SFA.DAS.Encoding", "{\"Encodings\": [{\"EncodingType\": \"AccountId\",\"Salt\": \"and vinegar\",\"MinHashLength\": 32,\"Alphabet\": \"46789BCDFGHJKLMNPRSTVWXY\"}]}"),
                new("DatabaseConnectionString", "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"),
                new("EmployerAccountsApi:IdentifierUri", "https://localhost:1"),
                new("EmployerAccountsApi:ApiBaseUrl", "https://localhost:4"),
                new("EmployerFinanceApi:IdentifierUri", "https://localhost:2"),
                new("EmployerFinanceApi:ApiBaseUrl", "https://localhost:3"),
                new("EnvironmentName", "LOCAL"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}
