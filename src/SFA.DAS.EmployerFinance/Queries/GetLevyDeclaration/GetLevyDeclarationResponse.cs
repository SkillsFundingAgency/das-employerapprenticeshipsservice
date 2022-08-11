
using SFA.DAS.EmployerFinance.Models.Levy;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationResponse
    {
        public List<LevyDeclarationItem> Declarations { get; set; }
    }
}
