using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    /// <summary>
    ///     This is a dummy implementation that will prevent restarting from working. It won't cause the
    ///     account processing to fail, but if it should need to restart it will always restart from 0.
    /// </summary>
    public class CheckpointDummy : ICheckpoint
    {
        //TODO: fill this in when we come back to this.
        public Task<long> GetLastCheckpointAsync(IIdProvider idProvider)
        {
            return Task.FromResult(0L);
        }

        public void SaveCheckpoint(IIdProvider idProvider, long id)
        {
            // not implemented
        }
    }
}