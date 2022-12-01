using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IUserAccountRepository
    {
        Task<Accounts<Account>> GetAccountsByUserRef(string userRef);
        Task<User> Get(string email);
        Task<User> Get(long id);
        Task<User> GetUserByRef(Guid @ref);
        Task Upsert(User user);
        Task<Accounts<Account>> GetAccounts();
    }
}
