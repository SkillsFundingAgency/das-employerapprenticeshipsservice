using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;
        
        public DasLevyService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ICollection<TransactionLine>> GetTransactionsByAccountId(long accountId)
        {
            var result = await _mediator.SendAsync(new GetAccountTransactionsRequest {AccountId = accountId});

            return result.TransactionLines;
        }

        public async Task<ICollection<AccountBalance>> GetAllAccountBalances()
        {
            var result = await _mediator.SendAsync(new GetAccountBalancesRequest());

            return result.Accounts;
        }

        public async Task<ICollection<T>> GetTransactionsByDateRange<T>(
            long accountId, DateTime fromDate, DateTime toDate, string externalUserId) where T : TransactionLine
        {
            var result = await _mediator.SendAsync(new GetAccountTransactionsByDateRangeQuery
            {
                AccountId = accountId,
                FromDate = fromDate,
                ToDate = toDate,
                ExternalUserId = externalUserId
            });

            return result?.Transactions?.OfType<T>().ToList() ?? new List<T>();
        }
    }
}