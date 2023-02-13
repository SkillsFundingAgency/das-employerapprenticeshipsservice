using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.Startup;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var hostBuilder = new HostBuilder();
            //try
            //{
            //    hostBuilder
            //        .UseDasEnvironment()
            //        .ConfigureDasAppConfiguration(args)
            //        .UseConsoleLifetime()
            //        .ConfigureLogging(b => b.AddNLog())
            //        .UseStructureMap()
            //        .ConfigureServices((c, s) => s
            //            .AddDasDistributedMemoryCache(c.Configuration, c.HostingEnvironment.IsDevelopment())
            //            .AddMemoryCache()
            //            .AddNServiceBus())
            //        .ConfigureContainer<Registry>(IoC.Initialize);

            //    using (var host = hostBuilder.Build())
            //    {
            //        await host.RunAsync();
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    throw;
            //}
        }
    }
}
