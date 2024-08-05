using NLog;
using NLog.Web;

namespace SFA.DAS.EAS.Support.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var logger = LogManager.Setup().LoadConfigurationFromFile(environment == "Development" ? "nlog.Development.config" : "nlog.config").GetCurrentClassLogger();
        logger.Info("Starting up host");
        
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseNLog();
            });
}
