using SFA.DAS.EmployerAccounts.Time;

namespace SFA.DAS.EmployerAccounts.Web;

public static class DateTimeRegistrationExtensions
{
    public static void AddDateTimeServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudCurrentTime = configuration.GetValue<string>("CurrentTime");

        if (!DateTime.TryParse(cloudCurrentTime, out var currentTime))
        {
            currentTime = DateTime.Now;
        }

        services.AddSingleton<ICurrentDateTime>(_ => new CurrentDateTime(currentTime));

     }
}