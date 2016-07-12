using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationResponse
    {
        public EnglishFractionDeclarations Fractions { get; set; }
        public Declarations Declarations { get; set; }
        public string Empref { get; set; }
    }
}