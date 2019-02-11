using SFA.DAS.EmployerFinance.Models.AccountTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IMembershipRepository
    {
        Task<TeamMember> Get(long accountId, string email);
        Task<Membership> Get(long userId, long accountId);
        Task Remove(long userId, long accountId);
        Task ChangeRole(long userId, long accountId, short Role);
        Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId);
        Task<MembershipView> GetCaller(long accountId, string externalUserId);
        Task Create(long userId, long accountId, short Role);
        Task SetShowAccountWizard(string hashedAccountId, string externalUserId, bool showWizard);
    }
}
