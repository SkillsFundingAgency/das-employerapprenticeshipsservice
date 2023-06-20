using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Extensions;
using SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness;

public static class Program
{
    public static async Task Main()
    {
        var provider = RegisterServices();

        await provider.GetService<PublishCreateAccountUserEvents>().Run();
        await provider.GetService<PublishCreatedAccountEvents>().Run();
    }

    private static IServiceProvider RegisterServices()
    {
        var configuration = GenerateConfiguration();
        var accountsConfiguration = configuration
            .GetSection(ConfigurationKeys.EmployerAccounts)
            .Get<EmployerAccountsConfiguration>();

        return new ServiceCollection()
            .AddNServiceBus()
            .AddSingleton<PublishCreateAccountUserEvents>()
            .AddSingleton<PublishCreatedAccountEvents>()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton(accountsConfiguration)
            .BuildServiceProvider();
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("SFA.DAS.EmployerAccounts:DatabaseConnectionString", "Data Source=.;Initial Catalog=SFA.DAS.EmployerAccounts;Integrated Security=True;Pooling=False;Connect Timeout=30"),
                new("SFA.DAS.EmployerAccounts:ServiceBusConnectionString", "test"),
                new("SFA.DAS.EmployerAccounts:NServiceBusLicense", "test"),
                new("EnvironmentName", "LOCAL"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider
        });
    }
}