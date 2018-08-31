using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions
{
    /// <summary>
    ///  AML-2454: Move to finance
    /// </summary>
    public class FindEmployerAccountLevyDeclarationTransactionsResponse
    {
        public List<LevyDeclarationTransactionLine> Transactions { get; set; }
        public decimal Total { get; set; }
    }
}