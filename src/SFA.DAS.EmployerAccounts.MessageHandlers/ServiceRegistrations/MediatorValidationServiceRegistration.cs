using System.Linq;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.ServiceRegistrations;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorValidators(this IServiceCollection services)
    {
        // Get the assembly that contains the validator type
        var validatorAssembly = typeof(CreateUserAccountCommandValidator).Assembly;

        // Register all validators in the assembly
        var validatorTypes = validatorAssembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddTransient(validatorInterface, validatorType);
        }
       
        return services;
    }
}