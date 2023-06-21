using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.ServiceRegistrations;

namespace SFA.DAS.EAS.Support.Web.Tests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(AccountController))]
    [TestCase(typeof(ChallengeController))]
    [TestCase(typeof(SearchController))]
    [TestCase(typeof(StatusController))]
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
        services.AddApplicationServices();
        services.AddConfigurationSections(config);
        services.AddApiClientServices();
        services.AddRepositories();
        
        services.AddLogging();

        services.AddTransient<AccountController>();
        services.AddTransient<ChallengeController>();
        services.AddTransient<SearchController>();
        services.AddTransient<StatusController>();
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("EnvironmentName", "LOCAL"),
                new("HashingService:Hashstring", "SaltAndVinegar"),
                new("HashingService:AllowedCharacters", "0123456789QWERTYUIOPASDFGHJKLZXCVBNM"),
                new("LevySubmission:TokenServiceApi:ApiBaseUrl", "https://localhost:123"),
                new("LevySubmission:TokenServiceApi:IdentifierUri", "https://localhost:321"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}