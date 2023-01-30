using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;
using SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorValidation(this IServiceCollection services)
    {
        services.AddTransient<IValidator<GetPayeSchemeByRefQuery>, GetPayeSchemeByRefValidator>();
        services.AddTransient<IValidator<GetEmployerAccountDetailByHashedIdQuery>, GetEmployerAccountDetailByHashedIdValidator>();
        services.AddTransient<IValidator<GetPagedEmployerAccountsQuery>, GetPagedEmployerAccountsValidator>();
        services.AddTransient<IValidator<GetTeamMembersRequest>, GetTeamMembersRequestValidator>();
        services.AddTransient<IValidator<GetTeamMembersWhichReceiveNotificationsQuery>, GetTeamMembersWhichReceiveNotificationsQueryValidator>();
        services.AddTransient<IValidator<GetAccountPayeSchemesQuery>, GetAccountPayeSchemesQueryValidator>();
        services.AddTransient<IValidator<GetEmployerAgreementByIdRequest>, GetEmployerAgreementByIdRequestValidator>();
        services.AddTransient<IValidator<GetMinimumSignedAgreementVersionQuery>, GetMinimumSignedAgreementVersionQueryValidator>();
        
        return services;
    }
}