using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(long id);
        Task<User> GetUserByRef(Guid @ref);
        Task<User> GetUserByRef(string id);
        Task<User> GetByEmailAddress(string emailAddress);
        Task Create(User registerUser);
        Task Update(User user);
        Task Upsert(User user);
        Task<Users> GetAllUsers();
    }
}
