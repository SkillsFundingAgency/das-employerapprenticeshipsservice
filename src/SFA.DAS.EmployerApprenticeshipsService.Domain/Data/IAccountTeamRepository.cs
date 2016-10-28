using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IAccountTeamRepository
    {
        Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedId, string externalUserId);
        Task<TeamMember> GetMember(string hashedId, string email);
    }
}
