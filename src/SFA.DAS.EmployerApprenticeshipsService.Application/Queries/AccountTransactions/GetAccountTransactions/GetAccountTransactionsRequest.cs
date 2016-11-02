using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions
{
    public class GetAccountTransactionsRequest : IAsyncRequest<GetAccountTransactionsResponse>, IAsyncRequest
    {
        public long AccountId { get; set; }
    }
}
