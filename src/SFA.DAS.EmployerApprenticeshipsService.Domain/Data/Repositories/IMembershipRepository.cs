using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IMembershipRepository
    {
        Task<TeamMember> Get(long accountId, string email);
        Task<Membership> Get(long userId, long accountId);
        Task Remove(long userId, long accountId);
        Task ChangeRole(long userId, long accountId, Role role);
        Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId);
        Task<MembershipView> GetCaller(long accountId, string externalUserId);
        Task Create(long userId, long accountId, Role role);
        Task SetShowAccountWizard(string hashedAccountId, string externalUserId, bool showWizard);
    }
}