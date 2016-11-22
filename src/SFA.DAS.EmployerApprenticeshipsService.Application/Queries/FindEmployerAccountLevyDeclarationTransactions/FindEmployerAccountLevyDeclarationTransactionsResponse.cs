using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions
{
    public class FindEmployerAccountLevyDeclarationTransactionsResponse
    {
        public List<LevyDeclarationTransactionLine> Transactions { get; set; }
        public decimal Total { get; set; }
    }
}