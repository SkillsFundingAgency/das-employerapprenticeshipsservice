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

            //// Aggregate by submission ID
            var balance = 0m;
            var transactionSubmissionGroups = response.OrderBy(c => c.TransactionDate)
                .GroupBy(c => new { c.SubmissionId }, (submission, group) => new
                {
                    submission.SubmissionId,
                    Data = group.ToList()
                });

            var transactionSubmissions = transactionSubmissionGroups.Select(item =>
            {
                var amount = item.Data.Sum(c => c.Amount);
                var transactionDate = item.Data.First().TransactionDate;
                var empRef = item.Data.First().EmpRef;

                return new
                {
                    Period = $"{transactionDate.Month}/{transactionDate.Year}",
                    Transaction = new TransactionLine
                    {
                        SubmissionId = item.SubmissionId,
                        SubTransactions = item.Data,
                        Amount = amount,
                        Description = amount >= 0 ? "Credit" : "Adjustment",
                        TransactionDate = transactionDate,
                        Balance = balance += amount,
                        EmpRef = empRef
                    }
                };
            }).ToList();

            var transactionSummaries = transactionSubmissions.GroupBy(t => t.Period, (period, transactionPeriod) =>
            {
                var transactions = transactionPeriod.Select(x => x.Transaction).ToList();
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
            }).OrderByDescending(t => t.TransactionDate)
                .ToList();

            return GetResponse(message.HashedId, message.AccountId, transactionSummaries);
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