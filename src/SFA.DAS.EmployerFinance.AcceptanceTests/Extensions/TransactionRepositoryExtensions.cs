using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class TransactionRepositoryExtensions
    {

        /// <summary>
        ///     Returns a task that polls the transaction table for the specified account's transactions.
        ///     The task completes when there have been <see cref="numberOfSteadyReadsThatIndicateCompleted"/> polls
        ///     without a change in the number of transactions found (indicating that the writes have completed).
        ///     <see cref="cancellationToken"/> can be used to terminate the polls early.
        /// </summary>
        public static async Task<bool> WaitForAllTransactionLinesInDatabase(
            this ITransactionRepository transactionRepository, 
            Account account, 
            CancellationToken cancellationToken,
            int pollIntervalMsecs = 1000,
            int numberOfSteadyReadsThatIndicateCompleted = 3)
        {
            return await WaitForStableResult(
                () => transactionRepository.GetPreviousTransactionsCount(account.Id, DateTime.MaxValue),
                cancellationToken, 1000, 3);
        }


        /// <summary>
        ///     Returns true if the supplied delegate returns the same value <see cref="requiredStableReadCount"/>
        ///     number of times. 
        /// </summary>
        /// <param name="resultGetter">Delegate that returns a task of int.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="pollIntervalMsecs">Frequency that <see cref="resultGetter"/> will be called.</param>
        /// <param name="requiredStableReadCount">
        ///     The number of times that the same result must be received to indicate that the result is stable.</param>
        /// <returns></returns>
        public static async Task<bool> WaitForStableResult(
                Func<Task<int>> resultGetter,
                CancellationToken cancellationToken,
                int pollIntervalMsecs,
                int requiredStableReadCount)
        {
            if (requiredStableReadCount < 2)
            {
                throw new InvalidOperationException($"Cannot establish whether a sequence of results is stable with fewer than 2 reads - value{requiredStableReadCount} is invalid");
            }

            var actualStableReadCount = 0;
            var result = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested && actualStableReadCount < requiredStableReadCount)
                {
                    var isSteady = await Task.Delay(pollIntervalMsecs, cancellationToken)
                        .ContinueWith(t => IsResultStable(resultGetter, ref result, cancellationToken),
                            cancellationToken);

                    // Note that when IsResultStable returns true we have already read the same result twice so the first true result means we have a sequence of 2, not 1.
                    actualStableReadCount = isSteady ? (actualStableReadCount == 0 ? 2 : actualStableReadCount + 1) : 0;
                }
            }
            catch (TaskCanceledException)
            {
                // sink exception
            }

            return actualStableReadCount == requiredStableReadCount;
        }

        private static bool IsResultStable(
            Func<Task<int>> resultGetter, 
            ref int count,
            CancellationToken cancellationToken)
        {
            var oldCount = count;
            var task = resultGetter();

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            task.Wait(cancellationToken);

            if (task.IsCanceled)
            {
                return false;
            }

            count = task.Result;

            return count > 0 && count == oldCount;
        }
    }
}
