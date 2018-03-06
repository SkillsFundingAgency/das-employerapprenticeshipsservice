using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    public class ProcessingInfo
    {
        public ProcessingInfo()
        {
            UnhandledExceptions = new List<string>();
        }

        public long LastProcessedId { get; set; }
        public int BatchSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int IdsProcessed { get; set; }
        public int BatchesProcessed { get; set; }
        public ProcessingState State { get; set; }
        public List<string> UnhandledExceptions { get; }
    }
}