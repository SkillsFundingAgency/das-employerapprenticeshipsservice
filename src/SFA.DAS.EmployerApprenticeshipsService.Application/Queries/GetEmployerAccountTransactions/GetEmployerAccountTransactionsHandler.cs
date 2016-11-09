using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsHandler : IAsyncRequestHandler<GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private readonly IDasLevyService _dasLevyService;
        private readonly IValidator<GetEmployerAccountTransactionsQuery> _validator;
        private readonly IEmployerAccountRepository _employerAccountRepository;

        public GetEmployerAccountTransactionsHandler(IDasLevyService dasLevyService, IValidator<GetEmployerAccountTransactionsQuery> validator, IEmployerAccountRepository employerAccountRepository)
        {
            _dasLevyService = dasLevyService;
            _validator = validator;
            _employerAccountRepository = employerAccountRepository;
        }

        public async Task<GetEmployerAccountTransactionsResponse> Handle(GetEmployerAccountTransactionsQuery message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var response = await _dasLevyService.GetTransactionsByAccountId(message.AccountId);

            var balance = 0m;
            var transactionSummary = response.OrderBy(c=>c.TransactionDate).GroupBy(c => new { c.SubmissionId}, (submission, group) => new
            {
                submission.SubmissionId,
                Data = group.ToList()
            }).Select(item =>
            {
                var amount = item.Data.Sum(c => c.Amount);
                var transactionDate = item.Data.First().TransactionDate;
                return new TransactionLine
                {
                    SubmissionId = item.SubmissionId,
                    SubTransactions = item.Data,
                    Amount = amount,
                    Description = amount>=0 ? "Credit":"Adjustment",
                    TransactionDate = transactionDate,
                    Balance = balance += amount
            };
            }).OrderBy(x => x.TransactionDate).ToList();
            
            var history = await _employerAccountRepository.GetAccountHistory(message.AccountId);

            var payeSchemeEndDate = DateTime.MinValue;
            history.ForEach(x =>
            {
                var transactions = transactionSummary.Where(t => t.TransactionDate < x.DateAdded &&
                                               t.TransactionDate > payeSchemeEndDate).ToList();

                if (transactions.Any())
                {
                    var aggregateTransaction = new TransactionLine
                    {
                        AccountId = x.AccountId,
                        EmpRef = x.PayeRef,
                        TransactionDate = x.DateAdded,
                        SubTransactions = transactions,
                        Amount = transactions.Sum(t => t.Amount),
                        Balance = transactions.OrderBy(t => t.TransactionDate).Last().Balance
                    };


                    var lastIndex = transactionSummary.IndexOf(transactions.Last());

                    transactionSummary.Insert(lastIndex, aggregateTransaction);

                    transactions.ForEach(t => transactionSummary.Remove(t));

                    payeSchemeEndDate = x.DateRemoved;
                }
            });

            transactionSummary.Reverse();

            var returnValue = new AggregationData
            {
                HashedId = message.HashedId,
                AccountId = message.AccountId,
                TransactionLines = transactionSummary.ToList()
            };
            return new GetEmployerAccountTransactionsResponse {Data = returnValue };
        }
    }
}
