using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.EmployerAccounts.ServiceRegistration;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.StartupExtensions;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Temp.AppStart;

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
    //[TestCase(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var startup = new Startup(GenerateConfiguration(), new Mock<IWebHostEnvironment>().Object);
        var serviceCollection = new ServiceCollection();
        startup.ConfigureServices(serviceCollection);

        serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    [TestCase(typeof(IRequestHandler<GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>))]
    [TestCase(typeof(IRequestHandler<RenameEmployerAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>))]
    [TestCase(typeof(IRequestHandler<AddPayeToAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<CreateAccountCommand, CreateAccountCommandResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>))]
    [TestCase(typeof(IRequestHandler<CreateUserAccountCommand, CreateUserAccountCommandResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountPayeSchemesForAuthorisedUserQuery, GetAccountPayeSchemesResponse>))]
    [TestCase(typeof(IRequestHandler<GetMemberRequest, GetMemberResponse>))]
    [TestCase(typeof(IRequestHandler<AddPayeToAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetPayeSchemeByRefQuery, GetPayeSchemeByRefResponse>))]
    [TestCase(typeof(IRequestHandler<RemovePayeFromAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<GetEmployerEnglishFractionHistoryQuery, GetEmployerEnglishFractionHistoryResponse>))]
    [TestCase(typeof(IRequestHandler<GetTeamMemberQuery, GetTeamMemberResponse>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Handlers(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var config = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(mockHostingEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IMembershipRepository>());
        serviceCollection.AddSingleton(Mock.Of<IAccountRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerAccountRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerSchemesRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerAgreementRepository>());
        serviceCollection.AddSingleton(Mock.Of<IEmployerAccountTeamRepository>());
        serviceCollection.AddSingleton(Mock.Of<IPayeRepository>());
        serviceCollection.AddSingleton(Mock.Of<IUserAccountRepository>());
        serviceCollection.AddSingleton(Mock.Of<IGenericEventFactory>());
        serviceCollection.AddSingleton(Mock.Of<ILegalEntityEventFactory>());
        serviceCollection.AddSingleton(Mock.Of<IPayeSchemeEventFactory>());
        serviceCollection.AddSingleton(Mock.Of<IAccountEventFactory>());
        serviceCollection.AddSingleton(Mock.Of<IEventPublisher>());
        serviceCollection.AddSingleton(Mock.Of<IPayeSchemesWithEnglishFractionService>());
        serviceCollection.AddSingleton(Mock.Of<IOuterApiClient>());

        serviceCollection.AddConfigurationOptions(config);
        serviceCollection.AddMediatR(typeof(GetEmployerAccountByHashedIdQuery));
        serviceCollection.AddMediatorValidators();
        serviceCollection.AddLogging();
        serviceCollection.AddAutoMapper(typeof(Startup).Assembly);

        var employerAccountsConfiguration = config.Get<EmployerAccountsConfiguration>();
        serviceCollection.AddHashingServices(employerAccountsConfiguration);
        
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }


    [Test]
    public void Then_Resolves_Authorization_Handlers()
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var startup = new Startup(GenerateConfiguration(), new Mock<IWebHostEnvironment>().Object);
        var serviceCollection = new ServiceCollection();
        startup.ConfigureServices(serviceCollection);
        serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetServices(typeof(IAuthorizationHandler)).ToList();

        Assert.IsNotNull(type);
        type.Count.Should().Be(1);
        type.Should().ContainSingle(c => c.GetType() == typeof(EmployerAccountAuthorizationHandler));
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