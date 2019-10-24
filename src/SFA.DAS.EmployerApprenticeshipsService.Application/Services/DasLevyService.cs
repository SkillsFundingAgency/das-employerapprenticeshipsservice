using MediatR;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;
        private readonly ITransactionRepository _transactionRepository;

        public DasLevyService(IMediator mediator, ITransactionRepository transactionRepository)
        {
            _mediator = mediator;
            _transactionRepository = transactionRepository;
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

        public async Task<int> GetPreviousAccountTransaction(long accountId, DateTime fromDate)
        {
            var result = await _mediator.SendAsync(new GetPreviousTransactionsCountRequest
            {
                AccountId = accountId,
                FromDate = fromDate
            });

            return result.Count;
        }

        public Task<string> GetProviderName(int ukprn, long accountId, string periodEnd)
        {
            return _transactionRepository.GetProviderName(ukprn, accountId, periodEnd);
        }
    }
}