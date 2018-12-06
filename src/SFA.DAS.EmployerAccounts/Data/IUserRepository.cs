using System.Data.Entity;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IUserRepository
    {
        Task<User> GetUserByRef(string id);
    }
}
