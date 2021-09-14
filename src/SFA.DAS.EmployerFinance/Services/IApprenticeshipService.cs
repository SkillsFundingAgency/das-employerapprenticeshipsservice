using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Apprenticeships;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IApprenticeshipService
    {
        Task<IEnumerable<ApprenticeshipDetail>> GetApprenticeshipsFor(long accountId);
    }
}