using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IAccountTeamRepository
    {
        Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId);
        Task<TeamMember> GetMember(string hashedAccountId, string email);
    }
}
