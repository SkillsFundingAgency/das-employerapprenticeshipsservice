using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Queries.FindEmployerAccountLevyDeclarationTransactions
{
    public class FindEmployerAccountLevyDeclarationTransactionsResponse
    {
        public List<LevyDeclarationTransactionLine> Transactions { get; set; }
        public decimal Total { get; set; }
    }
}