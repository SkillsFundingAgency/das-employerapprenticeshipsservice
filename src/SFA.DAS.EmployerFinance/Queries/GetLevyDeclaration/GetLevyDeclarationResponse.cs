using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationResponse
    {
        public List<LevyDeclarationView> Declarations { get; set; }
    }
}