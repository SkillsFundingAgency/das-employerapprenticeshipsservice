using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests;

public class WhenAddingServicesToTheContainer
{
    private readonly ServiceCollection _serviceCollection = new();
    private ServiceProvider _provider;

    [SetUp]
    public void Setup()
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var startup = new Startup(GenerateConfiguration(), new Mock<IWebHostEnvironment>().Object);
        startup.ConfigureServices(_serviceCollection);
        _serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        _provider = _serviceCollection.BuildServiceProvider();
    }

    [TestCase(typeof(AccountsOrchestrator))]
    [TestCase(typeof(AgreementOrchestrator))]
    [TestCase(typeof(UsersOrchestrator))]
    //[TestCase(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var type = _provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
                {
                    new("EmployerAccountsConfiguration:DatabaseConnectionString", "test"),
                    new("AllowedHashstringCharacters", "ABCDEFGHJKLMN12345"),
                    new("PublicAllowedHashstringCharacters", "ABCDEFGHJKLMN12345"),
                    new("PublicHashstring", "ABCDEFGHJKLMN12345"),
                    new("PublicAllowedAccountLegalEntityHashstringCharacters", "ABCDEFGHJKLMN12345"),
                    new("PublicAllowedAccountLegalEntityHashstringSalt", "ABCDEFGHJKLMN12345"),
                    new("HashString", "ABC123"),
                    new("AllowedCharacters", "ABC123"),
                    new("AccountApiConfiguration:ApiBaseUrl", "https://localhost:1"),
                    new("EmployerAccountsConfiguration:OuterApiApiBaseUri", "https://localhost:1"),
                    new("EmployerAccountsConfiguration:OuterApiSubscriptionKey", "test"),
                    new("ContentApi:ApiBaseUrl", "test"),
                    new("ContentApi:IdentifierUrl", "test"),
                    new("ProviderRegistrationsApi:BaseUrl", "test"),
                    new("ProviderRegistrationsApi:IdentifierUrl", "test"),
                    new("Environment", "test"),
                    new("EnvironmentName", "test"),
                    new("APPINSIGHTS_INSTRUMENTATIONKEY", "test"),
                    new("ElasticUrl", "test"),
                    new("ElasticUsername", "test"),
                    new("ElasticPassword", "test"),
                }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}