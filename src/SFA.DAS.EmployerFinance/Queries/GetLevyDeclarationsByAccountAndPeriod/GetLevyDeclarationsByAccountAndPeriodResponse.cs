using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclarationsByAccountAndPeriod
{
    public class GetLevyDeclarationsByAccountAndPeriodResponse
    {
        public List<LevyDeclarationView> Declarations { get; set; }
    }
}