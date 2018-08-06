using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Repositories
{
    public interface IUserAccountRepository
    {
        Task<Accounts<Account>> GetAccountsByUserRef(string userRef);
        Task<User> Get(string email);
        Task<User> Get(long id);
    }
}
