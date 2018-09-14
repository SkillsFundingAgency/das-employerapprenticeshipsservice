using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Data
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
