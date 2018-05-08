using SFA.DAS.EAS.Infrastructure.Exceptions;
using SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IExceptionMessageFormatter GetAppropriateExceptionFormatter(Exception exception)
        {
            return ExceptionMessageFormatterFactory.GetFormatter(exception);
        }

        public static IEnumerable<Exception> GetInnerExceptions(this Exception exception)
        {
            if (exception.InnerException == null) return new Exception[0];

            if (exception.GetType() != typeof(AggregateException))
            {
                return new[] { exception.InnerException };
            }

            var exceptions = ((AggregateException)exception).Flatten();

            return exceptions.InnerExceptions;
        }

        private static void GetExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            var messageFormatter = ExceptionMessageFormatterFactory.GetFormatter(exception);

            messageFormatter.AppendFormattedMessage(exception, messageBuilder);

            var innerExceptions = exception.GetInnerExceptions().ToArray();

            var hasMultipleInnerExceptions = innerExceptions.Length > 1;

            for (var index = 0; index < innerExceptions.Length; index++)
            {
                if (hasMultipleInnerExceptions)
                {
                    messageBuilder.AppendLine($"Exception: {index}");
                }

                GetExceptionMessage(innerExceptions[index], messageBuilder);
            }
        }
    }
}