using System;
using System.Linq;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution;
using SFA.DAS.EAS.Portal.Client.TestHarness.Scenarios;
using SFA.DAS.EAS.Portal.Client.TestHarness.Startup;
using SFA.DAS.EAS.Portal.Startup;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();

                var getAccount = host.Services.GetService<GetAccountScenario>();
                
                var accountTasks = Enumerable.Range(0, 9)
                    .AsParallel()
                    .Select(n => getAccount.Run());

                var accounts = await Task.WhenAll(accountTasks);

                var firstAccount = accounts.First();

                // display account
                Console.WriteLine(JsonConvert.SerializeObject(firstAccount));
                
                // assert all are identical
                var compareLogic = new CompareLogic(new ComparisonConfig {MaxDifferences = 100});
                foreach (var account in accounts.Skip(1))
                {
                    var comparisonResult = compareLogic.Compare(firstAccount, account);
                    if (!comparisonResult.AreEqual)
                        Console.WriteLine($"Not all accounts are identical: {comparisonResult.DifferencesString}");
                }
                
                await host.StopAsync();
            }            
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigurePortalClientConfiguration(args)
                .UseDasEnvironment()
                .UseStructureMap()
                .ConfigureServices(s => s.AddTransient<GetAccountScenario, GetAccountScenario>())
                .ConfigureContainer<Registry>(IoC.Initialize);
    }
}