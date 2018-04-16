using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Worker.IdProcessor
{
    public interface IProcessor
    {
        /// <summary>
        ///     Executed once per entity id.
        /// </summary>
        /// <param name="processingContext">Allows provider and processor to exchange settings.</param>
        /// <returns>true to continue otherwise false.</returns>
        Task<bool> DoAsync(long id, ProcessingContext processingContext);

        /// <summary>
        ///     Executed if an unhandled exception occurs whilst processing an entity. 
        /// </summary>
        /// <returns>
        ///     If this retyurns false then processing will continue. If false is returned then processing will be halted.
        /// </returns>
        Task<bool> InspectFailedAsync(long id, Exception exception, ProcessingContext processingContext);
    }
}