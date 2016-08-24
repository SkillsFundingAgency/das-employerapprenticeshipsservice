using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy
{
    public class EmployerLevyData
    {
        public string EmpRef { get; set; }

        public DasDeclarations Declarations { get; set; }

        public DasEnglishFractions Fractions { get; set; }
    }
}