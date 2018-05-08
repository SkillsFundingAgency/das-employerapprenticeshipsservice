using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class ExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public ExceptionMessageFormatter(Func<Exception, IExceptionMessageFormatter> getFormatterCallback)
            : base(getFormatterCallback)
        {
        }

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            if (exception == null)
            {
                messageBuilder.AppendLine("Exception is null");
            }
            else
            {
                messageBuilder.AppendLine($"Exception: {exception.GetType().Name}");
                messageBuilder.AppendLine($"Message: {exception.Message}");
            }
        }
    }
}
