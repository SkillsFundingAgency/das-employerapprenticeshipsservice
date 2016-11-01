using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IEmployerVerificationService
    {
        Task<EmployerInformation> GetInformation(string id);
    }
}