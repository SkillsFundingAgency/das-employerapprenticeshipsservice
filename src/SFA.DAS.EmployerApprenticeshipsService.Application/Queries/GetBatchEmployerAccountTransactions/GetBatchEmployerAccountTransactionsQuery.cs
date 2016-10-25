using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetBatchEmployerAccountTransactions
{
    public class GetBatchEmployerAccountTransactionsQuery : IAsyncRequest<GetBatchEmployerAccountTransactionsResponse>
    {
        public List<long> AccountIds { get; set; }
    }
}