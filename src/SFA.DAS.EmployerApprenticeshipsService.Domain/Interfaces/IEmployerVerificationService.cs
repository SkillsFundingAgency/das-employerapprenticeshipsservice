using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IEmployerVerificationService
    {
        Task<EmployerInformation> GetInformation(string id);
    }
}