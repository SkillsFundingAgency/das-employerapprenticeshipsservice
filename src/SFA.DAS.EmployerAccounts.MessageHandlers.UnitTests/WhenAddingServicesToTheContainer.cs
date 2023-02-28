using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using SFA.DAS.EmployerAccounts.MessageHandlers.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using HealthCheckEvent = SFA.DAS.EmployerAccounts.Messages.Events.HealthCheckEvent;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(IHandleMessages<AccountUserRemovedEvent>))]
    [TestCase(typeof(IHandleMessages<AccountUserRolesUpdatedEvent>))]
    [TestCase(typeof(IHandleMessages<CreatedAccountEvent>))]
    [TestCase(typeof(IHandleMessages<UserJoinedEvent>))]
    [TestCase(typeof(IHandleMessages<RefreshEmployerLevyDataCompletedEvent>))]
    [TestCase(typeof(IHandleMessages<RefreshPaymentDataCompletedEvent>))]
    [TestCase(typeof(IHandleMessages<AddedLegalEntityEvent>))]
    [TestCase(typeof(IHandleMessages<AddedPayeSchemeEvent>))]
    [TestCase(typeof(IHandleMessages<CreatedAccountEvent>))]
    [TestCase(typeof(IHandleMessages<CreatedAgreementEvent>))]
    [TestCase(typeof(IHandleMessages<DeletedPayeSchemeEvent>))]
    [TestCase(typeof(IHandleMessages<HealthCheckEvent>))]
    [TestCase(typeof(IHandleMessages<InvitedUserEvent>))]
    [TestCase(typeof(IHandleMessages<RemovedLegalEntityEvent>))]
    [TestCase(typeof(IHandleMessages<SignedAgreementEvent>))]
    [TestCase(typeof(IHandleMessages<UserJoinedEvent>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_EventHandlers(Type toResolve)
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
        var accountsConfiguration = configuration
            .GetSection(ConfigurationKeys.EmployerAccounts)
            .Get<EmployerAccountsConfiguration>();

        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplicationServices();
        services.AddUnitOfWork();
        services.AddNServiceBus();
        services.AddMemoryCache();
        services.AddCachesRegistrations();
        services.AddDatabaseRegistration(accountsConfiguration.DatabaseConnectionString);
        services.AddMediatR(
            typeof(UpdateAccountUserCommand),
            typeof(AcceptInvitationCommand)
        );

        RegisterEventHandlers(services);
    }

    private static void RegisterEventHandlers(IServiceCollection services)
    {
        var handlersAssembly = typeof(AccountUserRemovedEventHandler).Assembly;
        var handlerTypes = handlersAssembly
            .GetTypes()
            .Where(x => x.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>)));

        foreach (var handlerType in handlerTypes)
        {
           var handlerInterface = handlerType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>));
            services.AddTransient(handlerInterface, handlerType);
        }
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