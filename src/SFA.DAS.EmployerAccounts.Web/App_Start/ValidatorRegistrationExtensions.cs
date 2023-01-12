namespace SFA.DAS.EmployerAccounts.Web;

public  static class ValidatorRegistrationExtensions
{
    public static void AddValidators(this IServiceCollection services)
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .Single(assembly => assembly.GetName().Name.StartsWith(EmployerAccounts.Constants.ServiceNamespace));

        var validators = assembly
            .GetTypes()
            .Where(x => !x.IsInterface && x.GetInterface(typeof(IValidator<>).Name) != null);

        foreach (var validatorType in validators)
        {
            var type = validatorType.UnderlyingSystemType;
            services.AddTransient(type.GetInterface($"IValidator<{type.Name}>"), type);
        }
    }
}