using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IInvitationRepository
    {
        Task<List<InvitationView>> Get(string userId);
    }
}