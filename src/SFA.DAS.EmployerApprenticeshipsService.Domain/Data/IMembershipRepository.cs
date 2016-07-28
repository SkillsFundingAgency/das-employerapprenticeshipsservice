using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IMembershipRepository
    {
        Task<TeamMember> Get(long accountId, string email);
        Task<Membership> Get(long userId, long accountId);
        Task Remove(long userId, long accountId);
    }
}