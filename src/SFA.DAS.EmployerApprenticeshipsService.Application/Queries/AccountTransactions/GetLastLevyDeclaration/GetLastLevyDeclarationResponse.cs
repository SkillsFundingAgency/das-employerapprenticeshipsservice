using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationResponse
    {
        public DasDeclaration Transaction { get; set; }
    }
}