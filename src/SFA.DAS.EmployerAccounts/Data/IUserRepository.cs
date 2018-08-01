using SFA.DAS.EmployerAccounts.Models;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IUserRepository
    {
        Task Upsert(User user);
    }
}
