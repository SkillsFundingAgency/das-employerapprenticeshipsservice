using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.EAS.LevyAnalyser.Config;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.ResultSavers
{
    public class FileSummarySaver : ISummarySaver
    {
        private readonly IConfigProvider _configProvider;

        public FileSummarySaver(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public Task SaveAsync(AllAccountValidationResult results)
        {
            var config = _configProvider.Get<ResultFileSaverConfig>();

            var fileName = System.IO.Path.Combine(config.FolderName, $"ValidationResultSummary_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            var csv = CreateCsvOutput(results);

            System.IO.File.WriteAllText(fileName, csv);

            Console.WriteLine($"Saved output to {fileName}");

            return Task.CompletedTask;
        }

        private string CreateCsvOutput(AllAccountValidationResult results)
        {
            var invalidAccounts = results.Accounts.Where(x => !x.IsValid);
            var csvLines = new List<string>();
            csvLines.Add("AccountId, EmpRef, ExpectedAmount, ActualAmount, LatestTransactionDate");
            foreach (var account in invalidAccounts)
            {
                var failedRules = account.Rules.RuleEvaluationResults.Where(x => !x.IsValid);
                var messagesByEmpRef = GetMessagesByEmpRef(failedRules);

                foreach (var empRef in messagesByEmpRef)
                {
                    csvLines.Add(CreateCsvLine(account.AccountId, empRef));
                }
            }

            return string.Join("\n", csvLines);
        }

        private string CreateCsvLine(long accountId, IGrouping<string, RuleEvaluationMessage> empRef)
        {
            var lineItems = new List<string>();

            lineItems.Add(accountId.ToString());
            lineItems.Add(empRef.Key);
            lineItems.Add(empRef.Sum(x => x.ExpectedAmount).ToString());
            lineItems.Add(empRef.Sum(x => x.ActualAmount).ToString());
            lineItems.Add(empRef.Max(x => x.TransactionDate).ToString());
            
            return string.Join(",", lineItems);
        }

        private static IEnumerable<IGrouping<string, RuleEvaluationMessage>> GetMessagesByEmpRef(IEnumerable<IRuleEvaluationResult> failedRules)
        {
            var allMessages = new List<RuleEvaluationMessage>();
            foreach (var rule in failedRules)
            {
                allMessages.AddRange(rule.Messages);
            }

            var grouped = allMessages.Where(x => !string.IsNullOrEmpty(x.EmpRef)).GroupBy(x => x.EmpRef);
            return grouped;
        }

        private JsonSerializerSettings GetJsonSettings()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            };

            jsonSettings.Converters.Add(new StringEnumConverter());

            return jsonSettings;
        }
    }
}
