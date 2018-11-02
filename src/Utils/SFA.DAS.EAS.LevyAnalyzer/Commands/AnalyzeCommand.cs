using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
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
