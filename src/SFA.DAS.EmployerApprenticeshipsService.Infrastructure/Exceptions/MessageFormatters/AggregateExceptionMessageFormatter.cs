using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class AggregateExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(AggregateException);

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var exceptions = ((AggregateException)exception).Flatten();

            messageBuilder.AppendLine($"Aggregate exception has {exceptions.InnerExceptions.Count} inner exception.");
        }
    }
}
