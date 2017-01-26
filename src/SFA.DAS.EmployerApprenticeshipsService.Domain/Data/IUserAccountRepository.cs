using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IUserAccountRepository 
    {
        Task<Accounts<Account>> GetAccountsByUserId(string userId);
        Task<User> Get(string email);
        Task<User> Get(long id);
    }
}
