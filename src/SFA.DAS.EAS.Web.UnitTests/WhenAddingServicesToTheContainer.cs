using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Contracts.OuterApi;
using SFA.DAS.EAS.Application.ServiceRegistrations;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.StartupExtensions;

namespace SFA.DAS.EAS.Web.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(AccessDeniedController))]
    [TestCase(typeof(CookieConsentController))]
    [TestCase(typeof(EmployerAccountController))]
    [TestCase(typeof(EmployerAccountPayeController))]
    [TestCase(typeof(EmployerAgreementController))]
    [TestCase(typeof(EmployerTeamController))]
    [TestCase(typeof(HomeController))]
    [TestCase(typeof(InvitationController))]
    [TestCase(typeof(OrganisationController))]
    [TestCase(typeof(SearchOrganisationController))]
    [TestCase(typeof(SettingsController))]
    [TestCase(typeof(TransfersController))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Controllers(Type toResolve)
    {
        RunTestForType(toResolve);
    }

    [TestCase(typeof(IOuterApiClient))]
    [TestCase(typeof(IUserAccountService))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Services(Type toResolve)
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
        services.AddSingleton<IConfiguration>(config);
        services.AddSingleton(mockHostingEnvironment.Object);

        services.AddConfigurationSections(config);
        
        var easConfiguration = config.Get<EmployerApprenticeshipsServiceConfiguration>();
        
        services.AddOuterApiClient(easConfiguration.EmployerAccountsOuterApiConfiguration);
        services.AddAuthenticationServices();

        services.AddTransient<AccessDeniedController>();
        services.AddTransient<CookieConsentController>();
        services.AddTransient<EmployerAccountController>();
        services.AddTransient<EmployerAccountPayeController>();
        services.AddTransient<EmployerAgreementController>();
        services.AddTransient<EmployerTeamController>();
        services.AddTransient<HomeController>();
        services.AddTransient<InvitationController>();
        services.AddTransient<OrganisationController>();
        services.AddTransient<SearchOrganisationController>();
        services.AddTransient<SettingsController>();
        services.AddTransient<TransfersController>();
        
        services.AddTransient<IUserAccountService, UserAccountService>();
        services.AddTransient<IAccountClaimsService, AccountClaimsService>();
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("EmployerAccountsOuterApiConfiguration:BaseUrl", "https://localhost:321"),
                new("EmployerAccountsOuterApiConfiguration:Key", "ABC123"),
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}