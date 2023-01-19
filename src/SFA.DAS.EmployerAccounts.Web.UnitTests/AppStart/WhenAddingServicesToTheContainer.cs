using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(EmployerAccountOrchestrator))]
    [TestCase(typeof(EmployerAccountPayeOrchestrator))]
    [TestCase(typeof(EmployerAgreementOrchestrator))]
    [TestCase(typeof(EmployerTeamOrchestrator))]
    [TestCase(typeof(EmployerTeamOrchestratorWithCallToAction))]
    [TestCase(typeof(HomeOrchestrator))]
    [TestCase(typeof(InvitationOrchestrator))]
    [TestCase(typeof(OrganisationOrchestrator))]
    [TestCase(typeof(SearchOrganisationOrchestrator))]
    [TestCase(typeof(SearchPensionRegulatorOrchestrator))]
    [TestCase(typeof(SupportErrorOrchestrator))]
    [TestCase(typeof(TaskOrchestrator))]
    [TestCase(typeof(UserSettingsOrchestrator))]
    [TestCase(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    [Test]
    public void Then_Resolves_Authorization_Handlers()
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetServices(typeof(IAuthorizationHandler)).ToList();

        Assert.IsNotNull(type);
        type.Count.Should().Be(1);
        type.Should().ContainSingle(c => c.GetType() == typeof(EmployerAccountAuthorizationHandler));
    }

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        var EmployerAccountsConfiguration = configuration
            .GetSection("EmployerAccountsConfiguration")
            .Get<EmployerAccountsConfiguration>();
        var hostEnvironment = new Mock<IWebHostEnvironment>();
        serviceCollection.AddSingleton(hostEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
        serviceCollection.AddConfigurationOptions(configuration);
        serviceCollection.AddDistributedMemoryCache();
        serviceCollection.AddAuthenticationServices();
        serviceCollection.AddApplicationServices(EmployerAccountsConfiguration);
        serviceCollection.AddOrchestrators();

        serviceCollection.AddDatabaseRegistration(EmployerAccountsConfiguration, configuration["Environment"]);
        serviceCollection.AddLogging();
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
                {
                    new("EmployerAccountsConfiguration:DatabaseConnectionString", "test"),
                    new("EmployerAccountsConfiguration:AllowedCharacters", "ABCDEFGHJKLMN12345"),
                    new("EmployerAccountsConfiguration:HashString", "ABC123"),
                    new("AccountApiConfiguration:ApiBaseUrl", "https://localhost:1"),
                    new("EmployerAccountsConfiguration:OuterApiApiBaseUri", "https://localhost:1"),
                    new("EmployerAccountsConfiguration:OuterApiSubscriptionKey", "test"),
                    new("Environment", "test"),
                }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}