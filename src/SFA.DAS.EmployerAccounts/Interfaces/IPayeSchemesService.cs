using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IPayeSchemesService
    {
        Task<IEnumerable<PayeView>> GetPayeSchemes(string hashedAccountId);
    }
}