using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
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

        public async Task<ICollection<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await _mediator.SendAsync(new GetAccountTransactionsRequest
            {
                AccountId = accountId,
                FromDate = fromDate,
                ToDate = toDate
            });

            return result.TransactionLines;
        }

        public async Task<ICollection<AccountBalance>> GetAllAccountBalances()
        {
            var result = await _mediator.SendAsync(new GetAccountBalancesRequest());

            return result.Accounts;
        }

        public async Task<ICollection<T>> GetAccountProviderTransactionsByDateRange<T>(
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

        public async Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef)
        {
            var result = await _mediator.SendAsync(new GetEnglishFractionDetailByEmpRefQuery
                    {
                        EmpRef = empRef
                    });

            return result.FractionDetail;
        }

        public async Task<int> GetPreviousAccountTransaction(long accountId, DateTime fromDate, string externalUserId)
        {
            var result = await _mediator.SendAsync(new GetPreviousTransactionsCountRequest
            {
                AccountId = accountId,
                FromDate = fromDate,
                ExternalUserId = externalUserId
            });

            return result.Count;
        }

        public async Task<DasDeclaration> GetLastLevyDeclarationforEmpRef(string empRef)
        {
            var result = await _mediator.SendAsync(new GetLastLevyDeclarationQuery
            {
                EmpRef = empRef
            });

            return result.Transaction;
        }
    }
}