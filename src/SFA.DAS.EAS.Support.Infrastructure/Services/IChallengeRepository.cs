using SFA.DAS.EAS.Support.Infrastructure.Models;
using System.Threading.Tasks;


namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public interface IChallengeRepository
    {
        Task<bool> CheckData(Core.Models.Account record, ChallengePermissionQuery message);
    }
}