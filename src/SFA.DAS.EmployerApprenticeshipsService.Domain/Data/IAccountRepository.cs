using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IAccountRepository
    {
        Task CreateAccount(string userId, string employerNumber, string employerName, string employerRef);
    }
}