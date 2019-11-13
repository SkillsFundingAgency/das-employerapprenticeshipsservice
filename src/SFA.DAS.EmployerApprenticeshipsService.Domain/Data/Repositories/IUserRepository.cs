using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IUserRepository
    {
        Task Upsert(User user);
    }
}
