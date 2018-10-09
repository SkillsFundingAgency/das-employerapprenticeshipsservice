using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData
{
    /// <summary>
    ///     Represents a service that can take levy declarations and pre-process them to make them 
    ///     suitable for processing by EAS. 
    /// </summary>
    /// <remarks>
    ///     The pre-processing that occurs is to remove duplicate subsidy ids, remove declarations that 
    ///     are outside the period range and evaluate year end adjustments. 
    /// </remarks>
    public interface ILevyImportCleanerStrategy
    {
        Task<DasDeclaration[]> Cleanup(string empRef, IEnumerable<DasDeclaration> declarations);
    }
}