using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IInvitationRepository
    {
        Task<Invitations> Get(string userId);
    }
}