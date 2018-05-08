using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal abstract class BaseExceptionMessageFormatter : IExceptionMessageFormatter
    {
        protected readonly Func<Exception, IExceptionMessageFormatter> _getExecptionFormatterCallback;
        public virtual Type SupportedException => typeof(Exception);

        protected BaseExceptionMessageFormatter(Func<Exception, IExceptionMessageFormatter> getExecptionFormatterCallback)
        {
            _getExecptionFormatterCallback = getExecptionFormatterCallback;
        }

        public string GetFormattedMessage(Exception exception)
        {
            var messageBuilder = new StringBuilder();

            AppendFormattedMessage(exception, messageBuilder);

            return messageBuilder.ToString();
        }

        public void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            CreateFormattedMessage(exception, messageBuilder);
            CreateInnerExceptionMessages(exception, messageBuilder);
        }

        protected abstract void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder);

        protected virtual void CreateInnerExceptionMessages(Exception exception, StringBuilder messageBuilder)
        {
            if (exception.InnerException == null) return;

            var messageFormatter = _getExecptionFormatterCallback(exception.InnerException);
            messageFormatter.AppendFormattedMessage(exception.InnerException, messageBuilder);
        }
    }
}
