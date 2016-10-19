using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
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
