
using SFA.DAS.EmployerFinance.Models.Levy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationResponse
    {
        public List<LevyDeclarationView> Declarations { get; set; }
    }
}
