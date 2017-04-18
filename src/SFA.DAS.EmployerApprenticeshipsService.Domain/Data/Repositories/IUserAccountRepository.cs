using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IUserAccountRepository 
    {
        Task<Accounts<Account>> GetAccountsByUserRef(string userRef);
        Task<User> Get(string email);
        Task<User> Get(long id);
    }
}
