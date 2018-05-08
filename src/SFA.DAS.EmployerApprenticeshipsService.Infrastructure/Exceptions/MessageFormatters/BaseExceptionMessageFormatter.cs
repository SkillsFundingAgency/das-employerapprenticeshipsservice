using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal abstract class BaseExceptionMessageFormatter : IExceptionMessageFormatter
    {
        public virtual Type SupportedException => typeof(Exception);

        public string GetFormattedMessage(Exception exception)
        {
            var messageBuilder = new StringBuilder();

            AppendFormattedMessage(exception, messageBuilder);

            return messageBuilder.ToString();
        }

        public abstract void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder);
    }
}
