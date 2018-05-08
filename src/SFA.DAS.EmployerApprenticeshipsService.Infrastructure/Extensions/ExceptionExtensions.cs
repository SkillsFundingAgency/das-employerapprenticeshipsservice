using SFA.DAS.EAS.Infrastructure.Exceptions;
using SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters;
using System;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetMessage(this Exception exception)
        {
            const int reasonableInitialMessageSize = 200;
            var messageBuilder = new StringBuilder(reasonableInitialMessageSize);

            var currentException = exception;

            while (currentException != null)
            {
                var messageFormatter = ExceptionMessageFormatterFactory.GetFormatter(exception);

                messageFormatter.AppendFormattedMessage(exception, messageBuilder);

                currentException = currentException.InnerException;
            }

            var message = messageBuilder.ToString();

            return message;
        }

        public static IExceptionMessageFormatter GetAppropriateExceptionFormatter(Exception exception)
        {
            return ExceptionMessageFormatterFactory.GetFormatter(exception);
        }
    }
}