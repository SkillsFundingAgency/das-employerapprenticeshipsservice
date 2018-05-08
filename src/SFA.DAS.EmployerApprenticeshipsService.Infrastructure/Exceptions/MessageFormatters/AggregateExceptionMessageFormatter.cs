using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class AggregateExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        private readonly Func<Exception, IExceptionMessageFormatter> _getExecptionFormatterCallback;

        public override Type SupportedException => typeof(AggregateException);

        public AggregateExceptionMessageFormatter(Func<Exception, IExceptionMessageFormatter> getExecptionFormatterCallback)
        {
            _getExecptionFormatterCallback = getExecptionFormatterCallback;
        }

        public override void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var exceptions = ((AggregateException)exception).Flatten();

            messageBuilder.AppendLine($"Aggregate exception has {exceptions.InnerExceptions.Count} inner exception.");

            for (var i = 0; i < exceptions.InnerExceptions.Count; i++)
            {
                messageBuilder.Append("Exception:");
                messageBuilder.Append(i);

                var formatter = _getExecptionFormatterCallback(exceptions.InnerExceptions[i]);
                formatter.AppendFormattedMessage(exceptions.InnerExceptions[i], messageBuilder);
            }
        }
    }
}
