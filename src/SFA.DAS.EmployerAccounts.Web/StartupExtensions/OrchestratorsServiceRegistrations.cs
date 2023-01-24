namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class OrchestratorsServiceRegistrations
{
    public static IServiceCollection AddOrchestrators(this IServiceCollection services)
    {
        services.AddTransient<EmployerAccountOrchestrator>();
        services.AddTransient<EmployerAccountPayeOrchestrator>();
        services.AddTransient<EmployerAgreementOrchestrator>();
        services.AddTransient<EmployerTeamOrchestrator>();
        services.AddTransient<EmployerTeamOrchestratorWithCallToAction>();
        services.AddTransient<HomeOrchestrator>();
        services.AddTransient<InvitationOrchestrator>();
        services.AddTransient<OrganisationOrchestrator>();
        services.AddTransient<SearchOrganisationOrchestrator>();
        services.AddTransient<SearchPensionRegulatorOrchestrator>();
        services.AddTransient<SupportErrorOrchestrator>();
        services.AddTransient<TaskOrchestrator>();
        services.AddTransient<UserSettingsOrchestrator>();

        return services;
    }
}