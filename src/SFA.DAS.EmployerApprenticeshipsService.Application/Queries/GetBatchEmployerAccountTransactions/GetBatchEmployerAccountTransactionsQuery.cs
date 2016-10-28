using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetBatchEmployerAccountTransactions
{
    public class GetBatchEmployerAccountTransactionsQuery : IAsyncRequest<GetBatchEmployerAccountTransactionsResponse>
    {
        public List<long> AccountIds { get; set; }
    }
}