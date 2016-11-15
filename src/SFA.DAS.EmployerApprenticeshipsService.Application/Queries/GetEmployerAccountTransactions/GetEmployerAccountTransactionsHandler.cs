using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsHandler :
        IAsyncRequestHandler<GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private readonly IDasLevyService _dasLevyService;
        private readonly IValidator<GetEmployerAccountTransactionsQuery> _validator;

        public GetEmployerAccountTransactionsHandler(IDasLevyService dasLevyService,
            IValidator<GetEmployerAccountTransactionsQuery> validator)
        {
            _dasLevyService = dasLevyService;
            _validator = validator;
        }

        public async Task<GetEmployerAccountTransactionsResponse> Handle(GetEmployerAccountTransactionsQuery message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var response = await _dasLevyService.GetTransactionsByAccountId(message.AccountId);

            if (!response.Any())
            {
                return GetResponse(message.HashedId, message.AccountId);
            }

            var transactionSubmissions = GroupTransactionsBySubmissionId(response);
            var transactionSummaries = GroupTransactionsByPeriod(transactionSubmissions);

            return GetResponse(message.HashedId, message.AccountId, transactionSummaries);
        }

        private static ICollection<TransactionLine> GroupTransactionsByPeriod(
            IEnumerable<Tuple<string, TransactionLine>> transactionSubmissions)
        {
            var transations = transactionSubmissions.GroupBy(t => t.Item1, (period, transactionPeriod) =>
            {
                var transactions = transactionPeriod.Select(x => x.Item2).ToList();
                var combinedTotal = transactions.Sum(t => t.Amount);
                var accountId = transactions.First().AccountId;
                var payeSchemeRef = transactions.First().EmpRef;
                var transactionDate = transactions.Max(t => t.TransactionDate);
                var transactionBalance = transactions.OrderBy(t => t.TransactionDate).Last().Balance;

                return new TransactionLine
                {
                    AccountId = accountId,
                    EmpRef = payeSchemeRef,
                    Description = combinedTotal >= 0 ? "Credit" : "Adjustment",
                    TransactionDate = transactionDate,
                    SubTransactions = transactions,
                    Amount = combinedTotal,
                    Balance = transactionBalance
                };
            });

            return transations.OrderByDescending(t => t.TransactionDate).ToList();
        }
        
        private static IEnumerable<Tuple<string, TransactionLine>> GroupTransactionsBySubmissionId(
           IEnumerable<TransactionLine> response)
        {
            var orderedTransactions = response.OrderBy(c => c.TransactionDate);

            var transactionGroups = orderedTransactions.GroupBy(c => c.SubmissionId, (submission, group) =>
                new
                {
                    SubmissionId = submission,
                    Transactions = group.ToList()
                });

            var balance = 0m;

            return transactionGroups.Select(group =>
            {
                var amount = group.Transactions.Sum(c => c.Amount);
                var transactionDate = group.Transactions.First().TransactionDate;
                var empRef = group.Transactions.First().EmpRef;
                var transactionPeriod = $"{transactionDate.Month}/{transactionDate.Year}";

                var transaction = new TransactionLine
                {
                    SubmissionId = group.SubmissionId,
                    SubTransactions = group.Transactions,
                    Amount = amount,
                    Description = amount >= 0 ? "Credit" : "Adjustment",
                    TransactionDate = transactionDate,
                    Balance = balance += amount,
                    EmpRef = empRef
                };

                return new Tuple<string, TransactionLine>(transactionPeriod, transaction);
            }).ToList();
        }

        private static GetEmployerAccountTransactionsResponse GetResponse(string hashedAccountId, long accountId)
        {
            return GetResponse(hashedAccountId, accountId, new List<TransactionLine>());
        }

        private static GetEmployerAccountTransactionsResponse GetResponse(
            string hashedAccountId, long accountId, ICollection<TransactionLine> transactions)
        {
            return new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData
                {
                    HashedId = hashedAccountId,
                    AccountId = accountId,
                    TransactionLines = transactions
                }
            };
        }
    }
}