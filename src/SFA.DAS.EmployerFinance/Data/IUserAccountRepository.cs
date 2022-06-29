using SFA.DAS.EmployerFinance.Models.UserProfile;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserAccountRepository
    {
        Task Upsert(User user);
    }
}
