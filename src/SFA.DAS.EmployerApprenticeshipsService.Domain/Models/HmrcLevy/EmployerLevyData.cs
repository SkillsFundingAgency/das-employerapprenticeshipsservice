using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Models.HmrcLevy
{
    public class EmployerLevyData
    {
        public string EmpRef { get; set; }

        public DasDeclarations Declarations { get; set; }

        public DasEnglishFractions Fractions { get; set; }
    }
}