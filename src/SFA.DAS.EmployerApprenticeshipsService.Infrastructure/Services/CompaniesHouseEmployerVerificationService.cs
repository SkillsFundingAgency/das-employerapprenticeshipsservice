using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class CompaniesHouseEmployerVerificationService : IEmployerVerificationService
    {
        public Task<EmployerInformation> GetInformation(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}