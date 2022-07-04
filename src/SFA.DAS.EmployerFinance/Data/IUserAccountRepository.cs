using SFA.DAS.EmployerFinance.Models.UserProfile;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IUserAccountRepository
    {
        void Upsert(User user);
    }
}
