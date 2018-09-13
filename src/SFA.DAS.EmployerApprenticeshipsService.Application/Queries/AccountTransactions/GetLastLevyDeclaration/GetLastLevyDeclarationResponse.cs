using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationResponse
    {
        public DasDeclaration Transaction { get; set; }
    }
}