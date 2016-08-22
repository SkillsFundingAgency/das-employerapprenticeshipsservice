using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IUserAccountRepository 
    {
        Task<Accounts> GetAccountsByUserId(string userId);
        Task<User> Get(string email);
        Task<User> Get(long id);
    }
}
