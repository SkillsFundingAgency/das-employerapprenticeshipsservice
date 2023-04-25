using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Jobs.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using SFA.DAS.EmployerAccounts.ReadStore.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.ServiceRegistration;

namespace SFA.DAS.EmployerAccounts.Jobs.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(SeedAccountUsersJob))]
    [TestCase(typeof(CreateReadStoreDatabaseJob))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Jobs(Type toResolve)
    {
        var services = new ServiceCollection();
        SetupServiceCollection(services);
        var provider = services.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    private static void SetupServiceCollection(IServiceCollection services)
    {
        var configuration = GenerateStubConfiguration();
   
        services.AddConfigurationOptions(configuration);
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplicationServices();
        services.AddReadStoreServices();
        services.AddDatabaseRegistration();
        services.AddScoped<CreateReadStoreDatabaseJob>();
        services.AddScoped<SeedAccountUsersJob>();
        services.AddTransient<IRunOnceJobsService, RunOnceJobsService>();
    }

    private static IConfigurationRoot GenerateStubConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
                {
                    new("SFA.DAS.Encoding", "{\"Encodings\": [{\"EncodingType\": \"AccountId\",\"Salt\": \"and vinegar\",\"MinHashLength\": 32,\"Alphabet\": \"46789BCDFGHJKLMNPRSTVWXY\"}]}"),
                    new("SFA.DAS.EmployerAccounts:DatabaseConnectionString", "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true"),
                    new("SFA.DAS.EmployerAccounts.ReadStore:Uri", "http://test/"),
                    new("SFA.DAS.EmployerAccounts.ReadStore:AuthKey", "aGVsbG93b3JsZA=="),
                    new("AccountApiConfiguration:ApiBaseUrl", "https://localhost:1"),
                    new("Environment", "test"),
                    new("EnvironmentName", "test"),
                    new("APPINSIGHTS_INSTRUMENTATIONKEY", "test"),
                }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}