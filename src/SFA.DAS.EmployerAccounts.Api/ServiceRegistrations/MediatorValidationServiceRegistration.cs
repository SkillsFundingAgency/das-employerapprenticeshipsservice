using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorValidation(this IServiceCollection services)
    {
        //services.AddScoped(typeof(IValidator<GetPayeSchemeByRefQuery>), typeof(GetPayeSchemeByRefValidator));
        services.AddTransient<IValidator<GetPayeSchemeByRefQuery>, GetPayeSchemeByRefValidator>();
        
        return services;
    }
}