using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.UserProfile;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserRepository
    {
        Task Upsert(User user);
    }
}
