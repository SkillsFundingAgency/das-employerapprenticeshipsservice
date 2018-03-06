using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    /// <summary>
    ///     Provides the ids that identify some set of entities, e.g. set of accounts, set of legal entities that 
    ///     are to be processed by an <see cref="IId"/>.
    /// </summary>
    public interface IIdProvider
    {
        /// <summary>
        ///     Provides an ienumerable of ids that should be processed. The enumeration must be returned in id order.
        /// </summary>
        /// <param name="startAfterId">
        ///     Only ids greater then this should be returned. Items returned that are less than this will be ignored.
        /// </param>
        /// <param name="count">
        ///     The maximum number of items that should be returned. Additional items will be ignored and subsequent 
        ///     calls (if any) will start at the point as though only the requested number of rows had been provided.
        /// </param>
        /// <param name="processingContext">
        ///     This can be used by the <see cref="IIdProvider"/> and <see cref="IProcessor"/> to exchange 
        ///     settings. It is not used by the <see cref="IBroker"/>.
        /// </param>
        /// <returns>
        ///     An enumeration of ids.
        /// </returns>
        Task<IEnumerable<long>> GetIdsAsync(long startAfterId, int count, ProcessingContext processingContext);
    }
}