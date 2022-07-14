using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.UserProfile;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserAccountRepository
    {
        Task<Accounts<Account>> GetAccountsByUserRef(string userRef);
        Task<User> Get(string email);
        Task<User> Get(long id);
        Task<User> GetUserByRef(Guid @ref);
        Task<Users> GetAllUsers();
        Task Upsert(User user);
    }
}
