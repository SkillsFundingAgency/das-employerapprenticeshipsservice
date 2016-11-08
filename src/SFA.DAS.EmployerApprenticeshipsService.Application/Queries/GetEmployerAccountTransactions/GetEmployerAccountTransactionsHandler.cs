﻿using System.Collections.Generic;
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

        public GetEmployerAccountTransactionsHandler(IDasLevyService dasLevyService, IValidator<GetEmployerAccountTransactionsQuery> validator)
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

            var balance = 0m;
            var transactionSummary = response.OrderBy(c=>c.TransactionDate).GroupBy(c => new { c.SubmissionId}, (submission, group) => new
            {
                submission.SubmissionId,
                Data = group.ToList()
            }).Select(item =>
            {
                var amount = item.Data.Sum(c => c.Amount);
                return new TransactionSummary
                {
                    Id = item.SubmissionId.ToString(),
                    TransactionLines = item.Data,
                    Amount = amount,
                    Description = "Levy Credit",
                    TransactionDate = item.Data.First().TransactionDate,
                    Balance = balance += amount
            };
            }).OrderByDescending(c=>c.TransactionDate);
            
            var returnValue = new AggregationData
            {
                HashedId = message.HashedId,
                AccountId = message.AccountId,
                TransactionSummary = transactionSummary.ToList()
            };
            return new GetEmployerAccountTransactionsResponse {Data = returnValue };
        }
    }
}
