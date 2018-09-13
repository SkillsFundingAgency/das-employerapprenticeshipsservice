using System;
using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.FindEmployerAccountLevyDeclarationTransactions
{
    public class FindEmployerAccountLevyDeclarationTransactionsQuery : IAsyncRequest<FindEmployerAccountLevyDeclarationTransactionsResponse>
    {
        public string HashedAccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
