using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationResponse
    {
        public EnglishFractionDeclarations Fractions { get; set; }
        public LevyDeclarations LevyDeclarations { get; set; }
        public string Empref { get; set; }
    }
}