using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class ExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            if (exception == null)
            {
                messageBuilder.Append("Exception is null");
            }
            else
            {
                messageBuilder.Append("Exception:");
                messageBuilder.Append(exception.GetType().Name);
                messageBuilder.Append(" Message:");
                messageBuilder.Append(exception.Message);
            }
        }
    }
}
