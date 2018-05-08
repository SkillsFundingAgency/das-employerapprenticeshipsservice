using SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters;
using System;
using System.Collections.Concurrent;
using System.Linq;
using ExceptionMessageFormatter = SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters.ExceptionMessageFormatter;

namespace SFA.DAS.EAS.Infrastructure.Exceptions
{
    internal static class ExceptionMessageFormatterFactory
    {
        private static readonly ConcurrentDictionary<Type, IExceptionMessageFormatter> ExceptionMessageFormatterCache =
            new ConcurrentDictionary<Type, IExceptionMessageFormatter>();

        private static IExceptionMessageFormatter GeneralExceptionFormatter => new ExceptionMessageFormatter();

        private static readonly IExceptionMessageFormatter[] ExceptionFormatters =
        {
            new AggregateExceptionMessageFormatter(GetFormatter),
            new HttpExceptionMessageFormatter(),
        };

        public static IExceptionMessageFormatter GetFormatter(Exception exception)
        {
            return ExceptionMessageFormatterCache.GetOrAdd(exception.GetType(), GetExceptionFormatter(exception));
        }

        private static IExceptionMessageFormatter GetExceptionFormatter(Exception exception)
        {
            IExceptionMessageFormatter formatter = null;

            if (exception == null)
            {
                return GeneralExceptionFormatter;
            }

            var exceptionType = exception.GetType();

            while (formatter == null)
            {
                // convert to dictionary
                formatter = ExceptionFormatters.FirstOrDefault(ef => ef.SupportedException == exceptionType);

                if (formatter != null) continue;

                if (exceptionType.BaseType == null)
                {
                    return GeneralExceptionFormatter;
                }

                exceptionType = exceptionType.BaseType;
            }

            return formatter;
        }
    }
}
