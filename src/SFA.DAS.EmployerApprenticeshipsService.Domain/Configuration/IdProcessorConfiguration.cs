﻿namespace SFA.DAS.EAS.Domain.Configuration
{
    public class IdProcessorConfiguration
    {
        public const int DefaultBatchSize = 500;

        public IdProcessorConfiguration()
        {
            BatchSize = DefaultBatchSize;
        }
        public int BatchSize { get; set; }
    }
}