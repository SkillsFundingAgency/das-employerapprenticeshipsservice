using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class LevyDeclarationFileBasedService : FileSystemRepository, ILevyDeclarationService
    {

        private const string DeclarationDataFileName = "declaration_{0}";
        private const string EnglishFractionsDataFileName = "english_fractions_{0}";

        public LevyDeclarationFileBasedService() : base("levy_data")
        {

        }

        public async Task<LevyDeclarations> GetLevyDeclarations(string id)
        {
            return await ReadFileById<LevyDeclarations>(string.Format(DeclarationDataFileName, id.Replace("/","-")));
        }

        public async Task<EnglishFractionDeclarations> GetEnglishFraction(string id)
        {
            return await ReadFileById<EnglishFractionDeclarations>(string.Format(EnglishFractionsDataFileName, id.Replace("/", "-")));
        }
    }
}
