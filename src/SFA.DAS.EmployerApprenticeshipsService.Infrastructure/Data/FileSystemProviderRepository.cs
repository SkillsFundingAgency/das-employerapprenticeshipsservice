using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class FileSystemProviderRepository : FileSystemRepository, IProviderRepository
    {
        private const string DataFileName = "provider_data";

        public FileSystemProviderRepository() 
            : base("Providers")
        {
        }

        public async Task<Providers> GetAllProviders()
        {
            return await ReadFileById<Providers>(DataFileName);
        }
    }
}