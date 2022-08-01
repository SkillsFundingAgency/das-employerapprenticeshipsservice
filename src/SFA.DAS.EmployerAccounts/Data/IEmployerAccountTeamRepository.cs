using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerAccountTeamRepository
    {
        Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId);
        Task<TeamMember> GetMember(string hashedAccountId, string email, bool onlyIfMemberIsActive);
        Task<List<TeamMember>> GetAccountTeamMembers(string hashedAccountId);
    }
}
