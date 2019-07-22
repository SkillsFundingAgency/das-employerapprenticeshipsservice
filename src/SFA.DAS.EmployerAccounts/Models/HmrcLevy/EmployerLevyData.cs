using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Models.HmrcLevy
{
    public class EmployerLevyData
    {
        public EmployerLevyData()
        {
            Declarations = new DasDeclarations {Declarations = new List<DasDeclaration>()};
        }
        public string EmpRef { get; set; }

        public DasDeclarations Declarations { get; set; }
        
    }
}