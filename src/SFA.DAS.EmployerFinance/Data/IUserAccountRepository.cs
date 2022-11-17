using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.UserProfile;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserAccountRepository
    {
        Task<User> Get(Guid @ref);
    }
}
