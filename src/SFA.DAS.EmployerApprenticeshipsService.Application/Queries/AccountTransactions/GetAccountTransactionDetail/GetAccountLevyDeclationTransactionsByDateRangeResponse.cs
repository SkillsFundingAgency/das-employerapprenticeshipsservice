using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountLevyDeclationTransactionsByDateRangeResponse
    {
        public List<LevyDeclarationTransactionLine> Transactions { get; set; }
    }
}