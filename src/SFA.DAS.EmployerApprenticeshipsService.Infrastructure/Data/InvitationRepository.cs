using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class InvitationRepository : BaseRepository, IInvitationRepository
    {
        public InvitationRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Invitations> Get(string userId)
        {
            return new Invitations
            {
                InvitationList = new List<Invitation>()
            };
        }
    }
}