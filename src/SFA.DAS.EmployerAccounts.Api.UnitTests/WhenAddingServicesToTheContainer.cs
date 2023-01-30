using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;
using SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.ServiceRegistration;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(AccountsOrchestrator))]
    [TestCase(typeof(AgreementOrchestrator))]
    [TestCase(typeof(UsersOrchestrator))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var config = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockHostingEnvironment.Object);
        serviceCollection.AddMediatR(typeof(GetUserAccountsQuery));
        serviceCollection.AddAutoMapper(typeof(Startup).Assembly);
        serviceCollection.AddOrchestrators();
        serviceCollection.AddHashingServices(config.Get<EmployerAccountsConfiguration>());
        serviceCollection.AddLogging();
        var provider = serviceCollection.BuildServiceProvider();
        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    [TestCase(typeof(IRequestHandler<GetPayeSchemeByRefQuery, GetPayeSchemeByRefResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAccountDetailByHashedIdQuery, GetEmployerAccountDetailByHashedIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetPagedEmployerAccountsQuery, GetPagedEmployerAccountsResponse>))]
    [TestCase(typeof(IRequestHandler<GetTeamMembersRequest, GetTeamMembersResponse>))]
    [TestCase(typeof(IRequestHandler<GetTeamMembersWhichReceiveNotificationsQuery, GetTeamMembersWhichReceiveNotificationsQueryResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAgreementByIdRequest, GetEmployerAgreementByIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetMinimumSignedAgreementVersionQuery, GetMinimumSignedAgreementVersionResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Handlers(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var config = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockHostingEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IPayeSchemesService>());
        serviceCollection.AddSingleton(Mock.Of<IPayeRepository>());
        serviceCollection.AddSingleton(Mock.Of<IUserAccountRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerAccountRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerAgreementRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerAccountTeamRepository>());
        serviceCollection.AddApiConfigurationSections(config);
        serviceCollection.AddMediatR(typeof(GetAccountPayeSchemesQuery));
        serviceCollection.AddMediatorValidation();
        serviceCollection.AddLogging();
        serviceCollection.AddHashingServices(config.Get<EmployerAccountsConfiguration>());

        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
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

        return new ConfigurationRoot(new List<Microsoft.Extensions.Configuration.IConfigurationProvider> { provider });
    }
}