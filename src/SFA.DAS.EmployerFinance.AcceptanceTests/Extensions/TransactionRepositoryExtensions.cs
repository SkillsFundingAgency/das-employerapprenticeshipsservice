using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class TransactionRepositoryExtensions
    {
        public static async Task WaitForTransactionLinesInDatabase(this ITransactionRepository transactionRepository, Account account, int stepTimeout)
        {
            var cancel = new CancellationTokenSource(stepTimeout);

            while (true)
            {
                await Task.Delay(1000, cancel.Token);

                if (await transactionRepository.GetPreviousTransactionsCount(account.Id, DateTime.MaxValue) > 0)
                    break;
            }
        }
    }
}
