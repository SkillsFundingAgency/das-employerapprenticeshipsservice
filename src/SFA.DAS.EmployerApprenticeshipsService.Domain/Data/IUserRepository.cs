using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IUserRepository
    {
        Task<User> GetUserById(long id);
        Task<User> GetByUserRef(string id);
        Task<User> GetByEmailAddress(string emailAddress);
        Task Create(User registerUser);
        Task Update(User user);
        Task<Users> GetAllUsers();
    }
}
