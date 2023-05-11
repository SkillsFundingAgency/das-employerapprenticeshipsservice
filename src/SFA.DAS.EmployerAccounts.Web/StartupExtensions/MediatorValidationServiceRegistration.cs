using System.Reflection;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorValidators(this IServiceCollection services)
    {
        // Get the assembly that contains the validator type
        Assembly validatorAssembly = typeof(CreateAccountCommandValidator).Assembly;

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