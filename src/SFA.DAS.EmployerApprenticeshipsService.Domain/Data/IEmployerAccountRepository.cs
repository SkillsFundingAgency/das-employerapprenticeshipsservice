using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEmployerAccountRepository
    {
        Task<Account> GetAccountById(long id);
    }
}
