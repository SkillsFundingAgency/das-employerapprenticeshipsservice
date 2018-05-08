using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class AggregateExceptionMessageFormatter : BaseExceptionMessageFormatter
    {

        public override Type SupportedException => typeof(AggregateException);

        public AggregateExceptionMessageFormatter(Func<Exception, IExceptionMessageFormatter> getExecptionFormatterCallback) : base(getExecptionFormatterCallback)
        { }

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var exceptions = ((AggregateException)exception).Flatten();

            messageBuilder.AppendLine($"Aggregate exception has {exceptions.InnerExceptions.Count} inner exception.");

            for (var i = 0; i < exceptions.InnerExceptions.Count; i++)
            {
                messageBuilder.AppendLine($"Exception: {i} ");

                var formatter = _getExecptionFormatterCallback(exceptions.InnerExceptions[i]);
                formatter.AppendFormattedMessage(exceptions.InnerExceptions[i], messageBuilder);
            }
        }

        protected override void CreateInnerExceptionMessages(Exception exception, StringBuilder messageBuilder)
        {
            //We already handle inner exceptions above so we do not need to process them here
        }
    }
}
