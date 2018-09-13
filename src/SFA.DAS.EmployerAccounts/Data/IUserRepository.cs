using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IUserRepository
    {
        Task Upsert(User user);
    }
}
