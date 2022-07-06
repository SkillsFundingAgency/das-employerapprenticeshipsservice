using SFA.DAS.EmployerFinance.Models.UserProfile;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserRepository
    {
        Task Upsert(User user);
    }
}
