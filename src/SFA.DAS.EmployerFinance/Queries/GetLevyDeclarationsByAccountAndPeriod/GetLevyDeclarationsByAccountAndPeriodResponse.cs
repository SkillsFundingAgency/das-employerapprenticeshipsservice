using SFA.DAS.EmployerFinance.Models.Levy;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclarationsByAccountAndPeriod
{
    public class GetLevyDeclarationsByAccountAndPeriodResponse
    {
        public List<LevyDeclarationView> Declarations { get; set; }
    }
}
