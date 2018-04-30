using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IMembershipRepository
    {
        Task<TeamMember> Get(long accountId, string email);
        Task<Membership> Get(long userId, long accountId);
        Task Remove(long userId, long accountId);
        Task ChangeRole(long userId, long accountId, short roleId);
        Task<MembershipView> GetCaller(string hashedAccountId, Guid externalUserId);
        Task<MembershipView> GetCaller(long accountId, Guid externalUserId);
        Task Create(long userId, long accountId, short roleId);
        Task SetShowAccountWizard(string hashedAccountId, Guid externalUserId, bool showWizard);
    }
}