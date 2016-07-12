using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface ILevyDeclarationService
    {
        Task<Declarations> GetLevyDeclarations(string id);

        Task<EnglishFractionDeclarations> GetEnglishFraction(string id);
    }
}