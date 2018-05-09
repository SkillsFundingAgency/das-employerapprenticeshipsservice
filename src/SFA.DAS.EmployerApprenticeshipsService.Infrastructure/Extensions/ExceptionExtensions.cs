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

            GetExceptionMessage(exception, messageBuilder);

            return messageBuilder.ToString();
        }

        public static void AppendMessage(this Exception exception, StringBuilder messageBuilder)
        {
            GetExceptionMessage(exception, messageBuilder);
        }

        public static IExceptionMessageFormatter GetAppropriateExceptionFormatter(Exception exception)
        {
            return ExceptionMessageFormatterFactory.GetFormatter(exception);
        }

        private static void GetExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            var messageFormatter = ExceptionMessageFormatterFactory.GetFormatter(exception);

            messageFormatter.AppendFormattedMessage(exception, messageBuilder);
        }
    }
}