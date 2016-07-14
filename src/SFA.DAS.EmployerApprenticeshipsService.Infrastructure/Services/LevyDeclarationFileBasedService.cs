using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class LevyDeclarationFileBasedService : FileSystemRepository, ILevyDeclarationService
    {

        private const string DeclarationDataFileName = "declaration_{0}";
        private const string EnglishFractionsDataFileName = "english_fractions_{0}";

        public LevyDeclarationFileBasedService() : base("levy_data")
        {

        }

        public async Task<Declarations> GetLevyDeclarations(string id)
        {
            return await ReadFileById<Declarations>(string.Format(DeclarationDataFileName, id.Replace("/","-")));
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFraction(string id)
        {
            return await ReadFileById<EnglishFractionDeclarations>(string.Format(EnglishFractionsDataFileName, id.Replace("/", "-")));
        }
    }
}
