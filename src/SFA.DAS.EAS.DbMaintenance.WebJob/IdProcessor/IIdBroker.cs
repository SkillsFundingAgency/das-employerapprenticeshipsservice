using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    /// <summary>
    ///     Obtain ids and for each invoke a specified processor.
    /// </summary>
    public interface IIdBroker
    {
        /// <summary>
        ///     Fetch ids from <see cref="IdProvider"/> in batches and for each id found
        ///     invoke <see cref="IProcessor"/>. If the process should fail the process will 
        ///     automatically restart from where it failed, restarting at the account that was being processed
        ///     when the failure occurred.
        /// </summary>
        /// <param name="idProvider">This will provide the Ids of the entities that are to be processed.</param>
        /// <param name="idProcessor">This will perform the required process for each entity.</param>
        /// <returns>An object that details the results, such as status and number of entities processed.</returns>
        Task<ProcessingInfo> ProcessAsync(IIdProvider idProvider, IProcessor idProcessor);
    }
}