using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
    public class DasWebJobTraceWriter : TraceWriter
    {
        private readonly ILog _logger;

        public DasWebJobTraceWriter(TraceLevel level, ILog logger) : base(level)
        {
            _logger = logger;
        }

        public override void Trace(TraceEvent traceEvent)
        {
            _logger.Trace(traceEvent.Message);
        }
    }
}