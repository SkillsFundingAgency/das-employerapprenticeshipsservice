using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;
        
        public DasLevyService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<TransactionLine>> GetTransactionsByAccountId(long accountId)
        {
            var result = await _mediator.SendAsync(new GetAccountTransactionsRequest {AccountId = accountId});

            return result.TransactionLines;
        }

        public async Task<List<AccountBalance>> GetAllAccountBalances()
        {
            var result = await _mediator.SendAsync(new GetAccountBalancesRequest());

            return result.Accounts;
        }

        public async Task<List<TransactionLineDetail>>  GetTransactionDetailById(long id)
        {
            var result = await _mediator.SendAsync(new GetAccountTransactionDetailQuery {Id = id});

            return result.Data;
        }
    }
}