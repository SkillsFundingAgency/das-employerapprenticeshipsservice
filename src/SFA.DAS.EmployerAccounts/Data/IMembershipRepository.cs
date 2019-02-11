using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IMembershipRepository
    {
        Task<TeamMember> Get(long accountId, string email);
        Task<Membership> Get(long userId, long accountId);
        Task Remove(long userId, long accountId);
        Task ChangeRole(long userId, long accountId, short Role);
        Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId);
        Task<MembershipView> GetCaller(long accountId, string externalUserId);
        Task SetShowAccountWizard(string hashedAccountId, string externalUserId, bool showWizard);
    }
}