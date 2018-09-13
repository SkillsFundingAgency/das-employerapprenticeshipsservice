using SFA.DAS.EmployerFinance.Models.UserProfile;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserRepository
    {
        Task<User> GetUserByRef(Guid @ref);
        Task Upsert(User user);
    }
}
