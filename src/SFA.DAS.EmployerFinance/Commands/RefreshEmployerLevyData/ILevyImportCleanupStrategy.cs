using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData
{
    public interface ILevyImportCleanupStrategy
    {
        Task<IEnumerable<DasDeclaration>> Cleanup(string empRef, IEnumerable<DasDeclaration> declarations);
    }
}