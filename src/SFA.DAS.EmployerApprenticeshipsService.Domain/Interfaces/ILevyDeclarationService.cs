using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ILevyDeclarationService
    {
        Task<LevyDeclarations> GetLevyDeclarations(string id);

        Task<EnglishFractionDeclarations> GetEnglishFraction(string id);
    }
}