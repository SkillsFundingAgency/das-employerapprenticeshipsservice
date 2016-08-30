using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface ILevyDeclarationService
    {
        Task<LevyDeclarations> GetLevyDeclarations(string id);

        Task<EnglishFractionDeclarations> GetEnglishFraction(string id);
    }
}