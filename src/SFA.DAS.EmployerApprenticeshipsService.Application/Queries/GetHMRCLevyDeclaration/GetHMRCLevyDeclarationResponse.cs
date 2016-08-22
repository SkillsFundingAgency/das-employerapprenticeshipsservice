using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationResponse
    {
        public EnglishFractionDeclarations Fractions { get; set; }
        public Declarations Declarations { get; set; }
        public string Empref { get; set; }
    }
}