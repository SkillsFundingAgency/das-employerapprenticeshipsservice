using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.Rules.Infrastructure;

namespace SFA.DAS.EAS.LevyAnalyzer.Commands
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

            RuleEvaluationResult[] results = null;

            foreach (var accountId in NumberRange.ToInts(config.AccountIds))
            {
                var account = await _accountRepository.GetAccountAsync(accountId);

                Console.WriteLine($"Fetched account {accountId} ({account.LevyDeclarations.Length} levy declarations - {account.Transactions.Length} transactions)");

                results = _ruleRepository.ApplyAllRules(account).ToArray();
            }

            await _resultSaver.SaveAsync(results);
        }
    }
}
