using SFA.DAS.EAS.Finance.Api.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public interface IEmployerFinanceApiClient
    {
        Task HealthCheck();

        Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId);
    }
}