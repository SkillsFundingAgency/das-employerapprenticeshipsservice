using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IUserRepository : IRepository
    {
        Task<User> GetById(string id);
        Task<User> GetByEmailAddress(string emailAddress);
        Task Create(User registerUser);
        Task Update(User user);

        Task<Users> GetAllUsers();
    }
}
