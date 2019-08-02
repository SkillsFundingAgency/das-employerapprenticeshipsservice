﻿using System;
using System.Linq;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution;
using SFA.DAS.EAS.Portal.Client.TestHarness.Scenarios;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IContainer container;
            using (container = IoC.Initialize())
            {
                var getAccount = container.GetInstance<GetAccountScenario>();
                
                var accountTasks = Enumerable.Range(0, 20)
                    .AsParallel()
                    .Select(n => getAccount.Run());

                var accounts = await Task.WhenAll(accountTasks);

                var firstAccount = accounts.First();

                // display account
                //todo: need to serialize vacancies properties
                //? => https://mariusschulz.com/blog/conditionally-serializing-fields-and-properties-with-jsonnet
                Console.WriteLine(JsonConvert.SerializeObject(firstAccount));
                
                // assert all are identical
                var compareLogic = new CompareLogic(new ComparisonConfig {MaxDifferences = 100});
                foreach (var account in accounts.Skip(1))
                {
                    var comparisonResult = compareLogic.Compare(firstAccount, account);
                    if (!comparisonResult.AreEqual)
                        Console.WriteLine($"Not all accounts are identical: {comparisonResult.DifferencesString}");
                }
            }
        }
    }
}