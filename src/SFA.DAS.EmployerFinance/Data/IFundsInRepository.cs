using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IFundsInRepository
    {
        Task<IEnumerable<LevyFundsIn>> GetFundsIn();
    }
}
