using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(AccountsOrchestrator))]
    [TestCase(typeof(AgreementOrchestrator))]
    [TestCase(typeof(UsersOrchestrator))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IWebHostEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var config = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockHostingEnvironment.Object);
        serviceCollection.AddMediatR(typeof(GetUserAccountsQuery));
        serviceCollection.AddAutoMapper(typeof(Startup).Assembly);
        serviceCollection.AddApplicationServices();
        serviceCollection.AddApiConfigurationSections(config);
        serviceCollection.AddOrchestrators();
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
    [TestCase(typeof(IRequestHandler<GetAccountLegalEntitiesByHashedAccountIdRequest, GetAccountLegalEntitiesByHashedAccountIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAgreementsByAccountIdRequest, GetEmployerAgreementsByAccountIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserByEmailQuery, GetUserByEmailResponse>))]
    [TestCase(typeof(IRequestHandler<UpdateUserAornLockRequest, Unit>))]
    [TestCase(typeof(IRequestHandler<RemovePayeFromAccountCommand, Unit>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Handlers(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IWebHostEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var config = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockHostingEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IPayeSchemesService>());
        serviceCollection.AddSingleton(Mock.Of<IUserAornPayeLockService>());
        serviceCollection.AddSingleton(Mock.Of<IGenericEventFactory>());
        serviceCollection.AddSingleton(Mock.Of<IPayeSchemeEventFactory>());
        serviceCollection.AddSingleton(Mock.Of<IEventPublisher>());
        serviceCollection.AddDatabaseRegistration();
        serviceCollection.AddDataRepositories();
        serviceCollection.AddApplicationServices();
        serviceCollection.AddApiConfigurationSections(config);
        serviceCollection.AddMediatR(typeof(GetAccountPayeSchemesQuery));
        serviceCollection.AddMediatorValidators();
        serviceCollection.AddLogging();

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
                new("SFA.DAS.Encoding", "{\"Encodings\": [{\"EncodingType\": \"AccountId\",\"Salt\": \"and vinegar\",\"MinHashLength\": 32,\"Alphabet\": \"46789BCDFGHJKLMNPRSTVWXY\"}]}"),
                new("DatabaseConnectionString", "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"),
                new("AccountApiConfiguration:ApiBaseUrl", "https://localhost:1"),
                new("OuterApiApiBaseUri", "https://localhost:1"),
                new("OuterApiSubscriptionKey", "test"),
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