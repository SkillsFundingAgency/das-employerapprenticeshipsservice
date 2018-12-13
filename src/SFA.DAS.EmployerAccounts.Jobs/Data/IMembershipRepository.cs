using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Jobs.Data
{
    public interface IMembershipRepository
    {
        Task<IEnumerable<MembershipUser>> GetAllAccountUsers();
    }
}
