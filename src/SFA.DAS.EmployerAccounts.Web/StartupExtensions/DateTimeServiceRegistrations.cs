using SFA.DAS.EmployerAccounts.Time;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class DateTimeServiceRegistrations
{
    public static IServiceCollection AddDateTimeServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudCurrentTime = configuration.GetValue<string>("CurrentTime");

        if (!DateTime.TryParse(cloudCurrentTime, out var currentTime))
        {
            currentTime = DateTime.Now;
        }

        services.AddSingleton<ICurrentDateTime>(_ => new CurrentDateTime(currentTime));

        return services;
    }
}