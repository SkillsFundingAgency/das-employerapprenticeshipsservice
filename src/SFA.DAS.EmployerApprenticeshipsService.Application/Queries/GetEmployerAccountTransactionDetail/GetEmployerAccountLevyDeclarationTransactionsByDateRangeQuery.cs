using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountLevyDeclarationTransactionsByDateRangeQuery : IAsyncRequest<GetEmployerAccountLevyDeclarationTransactionsByDateRangeResponse>
    {
        public string HashedAccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
