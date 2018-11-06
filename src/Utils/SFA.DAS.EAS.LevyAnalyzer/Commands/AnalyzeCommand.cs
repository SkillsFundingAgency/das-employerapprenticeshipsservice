using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyser.Commands
{
    public class AnalyzeCommand : ICommand
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfigProvider _configProvider;
        private readonly IRuleRepository _ruleRepository;
        private readonly IResultSaver _resultSaver;

        public AnalyzeCommand(
            IAccountRepository accountRepository,
            IConfigProvider configProvider,
            IRuleRepository ruleRepository,
            IResultSaver resultSaver)
        {
            _accountRepository = accountRepository;
            _configProvider = configProvider;
            _ruleRepository = ruleRepository;
            _resultSaver = resultSaver;
        }

        public async Task DoAsync(CancellationToken cancellationToken)
        {
            var config = _configProvider.Get<AnalyzeCommandConfig>();

            var results = new AllAccountValidationResult
            {
                AccountIds = config.AccountIds
            };

            foreach (var accountId in NumberRange.ToInts(config.AccountIds))
            {
                var account = await _accountRepository.GetAccountAsync(accountId);

                Console.Write($"Fetched account {accountId} ({account.LevyDeclarations.Length} levy declarations - {account.Transactions.Length} transactions)...");

                var levyValidationResult = ValidateLevy(account);

                Console.WriteLine($"{(levyValidationResult.IsValid ? "Valid" : "Invalid")}");

                results.AddResult(levyValidationResult);
            }

            await _resultSaver.SaveAsync(results);
        }

        private AccountValidationResult ValidateLevy(Account account)
        {
            var result = new AccountValidationResult
            {
                AccountId = account.Id,
                Rules = _ruleRepository.ApplyAllRules(account).ToArray()
            };

            return result;
        }
    }
}
