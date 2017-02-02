using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Employer;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IEmployerVerificationService
    {
        Task<EmployerInformation> GetInformation(string id);
    }
}