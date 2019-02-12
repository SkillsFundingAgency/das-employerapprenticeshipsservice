using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules.Infrastructure;
using Exception = System.Exception;

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

            var accountIds = string.IsNullOrWhiteSpace(config.AccountIds) ? await _accountRepository.GetAllAccountIds() : NumberRange.ToLongs(config.AccountIds);

            var results = new AllAccountValidationResult
            {
                AccountIds = accountIds
            };

            foreach (var accountId in accountIds)
            {
                var account = await _accountRepository.GetAccountAsync(accountId);
                if (TryValidateAccount(account, out var levyValidationResult))
                {
                    results.AddResult(levyValidationResult);
                }
            }

            await _resultSaver.SaveAsync(results.Accounts.Where(a => !a.IsValid));
        }

        private bool TryValidateAccount(Account account, out AccountValidationResult validationResult)
        {
            try
            {
                validationResult = ValidateLevy(account);
            }
            catch (Exception e)
            {
                Console.Write($"Failed to validate account - {e.Message}");
                Console.Write($"Stack trace - {e.StackTrace}");
                _resultSaver.SaveAsync(account);
                validationResult = null;
            }

            return validationResult != null;
        }


        private AccountValidationResult ValidateLevy(Account account)
        {
            Console.Write($"Fetched account {account.Id} ({account.LevyDeclarations.Length} levy declarations - {account.Transactions.Length} transactions)...");

            var result = new AccountValidationResult
            {
                AccountId = account.AccountId,
                Rules = _ruleRepository.ApplyAllRules(account)
            };

            Console.WriteLine($"{(result.IsValid ? "Valid" : "Invalid")}");

            return result;
        }
    }
}
