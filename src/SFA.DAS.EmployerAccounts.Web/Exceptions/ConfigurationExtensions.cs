namespace SFA.DAS.EmployerAccounts.Web.Exceptions;

public static class ConfigurationExtensions
{
    public static T GetSection<T>(this IConfiguration configuration)
    {
        return configuration
            .GetSection(typeof(T).Name)
            .Get<T>();
    }
}