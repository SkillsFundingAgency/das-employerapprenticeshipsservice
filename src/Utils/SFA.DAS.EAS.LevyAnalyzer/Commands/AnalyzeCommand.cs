using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyzer.Commands
{
    public class AnalyzeCommand : ICommand
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfigProvider _configProvider;

        public AnalyzeCommand(
            IAccountRepository accountRepository, 
            IConfigProvider configProvider)
        {
            _accountRepository = accountRepository;
            _configProvider = configProvider;
        }

        public async Task DoAsync(CancellationToken cancellationToken)
        {
            var config = _configProvider.Get<AnalyzeCommandConfig>();

            foreach (var accountId in NumberRange.ToInts(config.AccountIds))
            {
                var account = await _accountRepository.GetAccountAsync(accountId);

                Console.WriteLine($"Fetched account {accountId} ({account.LevyDeclarations.Length} levy declarations - {account.Transactions.Length} transactions)");
            }
        }
    }
}
