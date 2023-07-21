using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class DataRepositoryServiceRegistrations
{
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountLegalEntityRepository, AccountLegalEntityRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IEmployerAccountRepository, EmployerAccountRepository>();
        services.AddScoped<IEmployerAccountTeamRepository, EmployerAccountTeamRepository>();
        services.AddScoped<IEmployerAgreementRepository, EmployerAgreementRepository>();
        services.AddScoped<IEmployerSchemesRepository, EmployerSchemesRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IPayeRepository, PayeRepository>();
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();

        return services;
    }
}