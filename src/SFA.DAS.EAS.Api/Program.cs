using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Account.Api.Extensions;


namespace SFA.DAS.EAS.Account.Api;


public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureDasAppConfiguration();
                webBuilder.UseStartup<Startup>();
            });
}


