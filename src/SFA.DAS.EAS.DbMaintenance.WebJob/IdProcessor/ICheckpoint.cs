using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    /// <summary>
    ///     Responsible for saving and providing access to restart information for the <see cref="Broker"/>.
    /// </summary>
    public interface ICheckpoint
    {
        /// <summary>
        ///     Returns the id of the entity that was last successfully processed for the 
        ///     specified processor.
        /// </summary>
        Task<long> GetLastCheckpointAsync(IIdProvider idProvider);

        /// <summary>
        ///     Saves thes specified account id as being successfully processed for the <see cref="idProvider"/>.
        /// </summary>
        void SaveCheckpoint(IIdProvider idProvider, long id);
    }
}