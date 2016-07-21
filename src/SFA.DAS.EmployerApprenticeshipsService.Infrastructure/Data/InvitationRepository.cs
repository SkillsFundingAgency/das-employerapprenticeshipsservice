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

        public async Task<List<InvitationView>> Get(string userId)
        {
            return new List<InvitationView>();
        }
    }
}