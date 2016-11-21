using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class FileSystemStandardsRepository : FileSystemRepository, IStandardsRepository
    {
        private const string fileName = "standards";

        public FileSystemStandardsRepository()
            : base("Standards")
        {
        }

        public async Task<Standard[]> GetAllAsync()
        {
            return await ReadFileById<Standard[]>(fileName);
        }

        public async Task<Standard> GetByCodeAsync(string code)
        {
            var standards = await ReadFileById<Standard[]>(fileName);

            return standards.SingleOrDefault(x => x.Id == code);
        }
    }
}