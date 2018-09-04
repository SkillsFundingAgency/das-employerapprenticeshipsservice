﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Exceptions;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountLevyTransactions
{
    public class GetAccountLevyTransactionsQueryHandler : IAsyncRequestHandler<GetAccountLevyTransactionsQuery, GetAccountLevyTransactionsResponse>
    {
        private readonly IValidator<GetAccountLevyTransactionsQuery> _validator;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IHmrcDateService _hmrcDateService;
        
        public GetAccountLevyTransactionsQueryHandler(IValidator<GetAccountLevyTransactionsQuery> validator, ITransactionRepository transactionRepository, IHmrcDateService hmrcDateService)
        {
            _validator = validator;
            _transactionRepository = transactionRepository;
            _hmrcDateService = hmrcDateService;
        }

    public async Task<GetAccountLevyTransactionsResponse> Handle(GetAccountLevyTransactionsQuery message)
    {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transactions = await _transactionRepository.GetAccountLevyTransactionsByDateRange(
                message.AccountId,
                message.FromDate,
                message.ToDate);

            foreach (var transaction in transactions)
            {
                if (!string.IsNullOrEmpty(transaction.PayrollYear) && transaction.PayrollMonth != 0)
                {
                    transaction.PayrollDate = _hmrcDateService.GetDateFromPayrollYearMonth(transaction.PayrollYear, transaction.PayrollMonth);
                }
            }

            return new GetAccountLevyTransactionsResponse { Transactions = transactions };
        }
    }
}
