using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;
using SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Commands.ResendInvitation;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;
using SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;
using SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerAccounts.Queries.GetInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisations;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;

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
    //[TestCase(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Orchestrators(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var startup = new Startup(GenerateStubConfiguration(), new Mock<IWebHostEnvironment>().Object);
        var serviceCollection = new ServiceCollection();
        startup.ConfigureServices(serviceCollection);

        serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    [TestCase(typeof(IRequestHandler<RenameEmployerAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>))]
    [TestCase(typeof(IRequestHandler<AddPayeToAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<CreateAccountCommand, CreateAccountCommandResponse>))]
    [TestCase(typeof(IRequestHandler<CreateUserAccountCommand, CreateUserAccountCommandResponse>))]
    [TestCase(typeof(IRequestHandler<AddPayeToAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<RemovePayeFromAccountCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<SignEmployerAgreementCommand, SignEmployerAgreementCommandResponse>))]
    [TestCase(typeof(IRequestHandler<RemoveLegalEntityCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<DeleteInvitationCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<ChangeTeamMemberRoleCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UpdateShowAccountWizardCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<CreateInvitationCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<RemoveTeamMemberCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<ResendInvitationCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UnsubscribeProviderEmailCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UpsertRegisteredUserCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UpdateTermAndConditionsAcceptedOnCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<AcceptInvitationCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UpdateOrganisationDetailsCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<DismissMonthlyTaskReminderCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UpdateUserNotificationSettingsCommand, Unit>))]
    [TestCase(typeof(IRequestHandler<UnsubscribeNotificationCommand, Unit>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Command_Handlers(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var startup = new Startup(GenerateStubConfiguration(), new Mock<IWebHostEnvironment>().Object);
        var serviceCollection = new ServiceCollection();
        startup.ConfigureServices(serviceCollection);

        serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }

    [TestCase(typeof(IRequestHandler<GetEmployerAccountByIdQuery, GetEmployerAccountByIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountPayeSchemesForAuthorisedUserQuery, GetAccountPayeSchemesResponse>))]
    [TestCase(typeof(IRequestHandler<GetMemberRequest, GetMemberResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAccountByIdQuery, GetEmployerAccountByIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetPayeSchemeByRefQuery, GetPayeSchemeByRefResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerEnglishFractionHistoryQuery, GetEmployerEnglishFractionHistoryResponse>))]
    [TestCase(typeof(IRequestHandler<GetTeamMemberQuery, GetTeamMemberResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>))]
    [TestCase(typeof(IRequestHandler<GetNextUnsignedEmployerAgreementRequest, GetNextUnsignedEmployerAgreementResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>))]
    [TestCase(typeof(IRequestHandler<GetSignedEmployerAgreementPdfRequest, GetSignedEmployerAgreementPdfResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountLegalEntityRemoveRequest, GetAccountLegalEntityRemoveResponse>))]
    [TestCase(typeof(IRequestHandler<GetOrganisationAgreementsRequest, GetOrganisationAgreementsResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAccountByIdQuery, GetEmployerAccountByIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetTeamMemberQuery, GetTeamMemberResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountStatsQuery, GetAccountStatsResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserByRefQuery, GetUserByRefResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountTasksQuery, GetAccountTasksResponse>))]
    [TestCase(typeof(IRequestHandler<GetInvitationRequest, GetInvitationResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountTeamMembersQuery, GetAccountTeamMembersResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserQuery, GetUserResponse>))]
    [TestCase(typeof(IRequestHandler<GetEmployerAccountByIdQuery, GetEmployerAccountByIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetReservationsRequest, GetReservationsResponse>))]
    [TestCase(typeof(IRequestHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse>))]
    [TestCase(typeof(IRequestHandler<GetSingleCohortRequest, GetSingleCohortResponse>))]
    [TestCase(typeof(IRequestHandler<GetVacanciesRequest, GetVacanciesResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserAccountRoleQuery, GetUserAccountRoleResponse>))]
    [TestCase(typeof(IRequestHandler<GetGatewayInformationQuery, GetGatewayInformationResponse>))]
    [TestCase(typeof(IRequestHandler<GetGatewayTokenQuery, GetGatewayTokenQueryResponse>))]
    [TestCase(typeof(IRequestHandler<GetHmrcEmployerInformationQuery, GetHmrcEmployerInformationResponse>))]
    [TestCase(typeof(IRequestHandler<GetNumberOfUserInvitationsQuery, GetNumberOfUserInvitationsResponse>))]
    [TestCase(typeof(IRequestHandler<GetProviderInvitationQuery, GetProviderInvitationResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserInvitationsRequest, GetUserInvitationsResponse>))]
    [TestCase(typeof(IRequestHandler<CreateOrganisationAddressRequest, CreateOrganisationAddressResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountLegalEntityRequest, GetAccountLegalEntityResponse>))]
    [TestCase(typeof(IRequestHandler<GetOrganisationByIdRequest, GetOrganisationByIdResponse>))]
    [TestCase(typeof(IRequestHandler<GetOrganisationsRequest, GetOrganisationsResponse>))]
    [TestCase(typeof(IRequestHandler<GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>))]
    [TestCase(typeof(IRequestHandler<GetPensionRegulatorRequest, GetPensionRegulatorResponse>))]
    [TestCase(typeof(IRequestHandler<GetOrganisationsByAornRequest, GetOrganisationsByAornResponse>))]
    [TestCase(typeof(IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsQueryResponse>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved_For_Query_Handlers(Type toResolve)
    {
        var mockHostingEnvironment = new Mock<IHostingEnvironment>();
        mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Test");

        var startup = new Startup(GenerateStubConfiguration(), new Mock<IWebHostEnvironment>().Object);
        var serviceCollection = new ServiceCollection();
        startup.ConfigureServices(serviceCollection);

        serviceCollection.AddSingleton(_ => mockHostingEnvironment.Object);
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.IsNotNull(type);
    }
    
    private static IConfigurationRoot GenerateStubConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
                {
                    new("SFA.DAS.Encoding", "{\"Encodings\": [{\"EncodingType\": \"AccountId\",\"Salt\": \"and vinegar\",\"MinHashLength\": 32,\"Alphabet\": \"46789BCDFGHJKLMNPRSTVWXY\"}]}"),
                    new("DatabaseConnectionString", "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true"),
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
                    new("CommitmentsApi:IdentifierUrl", "test"),
                    new("CommitmentsApi:ApiBaseUrl", "test"),
                    new("DefaultServiceTimeoutMilliseconds", "100"),
                    new("EmployerAccountsOuterApiConfiguration:BaseUrl", "https://test.test"),
                    new("EmployerAccountsOuterApiConfiguration:Key", "test"),
                    new("Hmrc:BaseUrl", "test"),
                    new("PensionRegulatorApi:IdentifierUri", "test"),
                    new("PensionRegulatorApi:BaseUrl", "test"),
                    new("RecruitApi:IdentifierUri", "test"),
                    new("AuditApi:BaseUrl", "https://test.test"),
                    new("AuditApi:IdentifierUri", "test"),
                    new("TokenServiceApi:ApiBaseUrl", "https://test.test"),
                    new("TokenServiceApi:ClientId", "test"),
                    new("TokenServiceApi:ClientSecret", "test"),
                    new("TokenServiceApi:IdentifierUrl", "https://test.test"),
                    new("TokenServiceApi:Tenant", "test"),
                    new("TasksApi:ApiBaseUrl", "https://test.test"),
                    new("TasksApi:IdentifierUrl", "https://test.test"),
                    new("ResourceEnvironmentName", "TEST"),
                    new("Identity:ClientId", "clientId"),
                    new("EventsApi:BaseUrl", "https://test.test"),
                    new("EventsApi:ClientToken", "CLIENT_TOKEN")
                }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}