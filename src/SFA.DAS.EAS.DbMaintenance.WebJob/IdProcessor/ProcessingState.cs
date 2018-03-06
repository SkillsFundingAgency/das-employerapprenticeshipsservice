namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    public enum ProcessingState
    {
        /// <summary>
        ///     The process is in an undefined state. 
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The processor is currently being provided ids to process.
        /// </summary>
        InProgress = 1,

        /// <summary>
        ///     The processor was successfully invoked for every id returned by the relevant id provider.
        /// </summary>
        Success = 2,

        /// <summary>
        ///     The processor requested early termination but did not cause an exception.
        /// </summary>
        TerminatedEarly = 3,

        /// <summary>
        ///     The processor was terminated prematurely becuase it generated an unhandled exception.
        /// </summary>
        TerminatedByErrorHandler = 100,

        /// <summary>
        ///     The ids provided by <see cref="IIdProvider"/> were not provided in ascending order. 
        /// </summary>
        IdsProvidedOutOfOrder = 101
    }
}