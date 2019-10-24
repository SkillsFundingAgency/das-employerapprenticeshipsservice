using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IMembershipRepository
    {
        Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId);
        Task<MembershipView> GetCaller(long accountId, string externalUserId);
    }
}