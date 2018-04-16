using System;

namespace SFA.DAS.EAS.Account.Worker.IdProcessor
{
    public class ProcessorException : Exception
    {
        public ProcessorException() : base()
        {
        }

        public ProcessorException(string message) : base(message)
        {

        }

        public ProcessorException(string message, ProcessingInfo processingInfo) : base(message)
        {
            ProcessingInfo = processingInfo;
        }

        public ProcessingInfo ProcessingInfo { get; set; }
    }
}
