using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Web;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public class ExceptionTypeFormatter 
        {
            public ExceptionTypeFormatter(Type exceptionType, Action<Exception, StringBuilder> build)
            {
                Build = build;
                SupportedException = exceptionType;
            }

            public Type SupportedException { get; }
            public Action<Exception, StringBuilder> Build { get; }
        }

        private static readonly ConcurrentDictionary<Type, ExceptionTypeFormatter> ExceptionTypeFormatterCache = new ConcurrentDictionary<Type, ExceptionTypeFormatter>();

        private static readonly ExceptionTypeFormatter LastChanceFormatter = new ExceptionTypeFormatter(typeof(Exception), BuildLastChanceMessage);

        private static readonly ExceptionTypeFormatter[] ExceptionFormatters = new ExceptionTypeFormatter[]
        {
            // This will catch all exceptions
            new ExceptionTypeFormatter(typeof(Exception), BuildExceptionMessage),
            new ExceptionTypeFormatter(typeof(AggregateException), BuildAggregateExceptionMessage),
            new ExceptionTypeFormatter(typeof(HttpException), BuildHttpExceptionMessage) 
        };


        public static string GetMessage(this Exception exception)
        {
            const int reasonableInitialMessageSize = 200;
            var messageBuilder = new StringBuilder(reasonableInitialMessageSize);

            BuildDetailedExceptionMessage(exception, messageBuilder);

            return messageBuilder.ToString();
        }

        private static void BuildDetailedExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            if (exception == null)
            {
                return;
            }

            var handler = ExceptionTypeFormatterCache.GetOrAdd(exception.GetType(), GetAppropriateExceptionFormatter(exception));

            handler.Build(exception, messageBuilder);

            BuildDetailedExceptionMessage(exception.InnerException, messageBuilder);
        }

        public static ExceptionTypeFormatter GetAppropriateExceptionFormatter(Exception exception)
        {
            ExceptionTypeFormatter result = null;

            if (exception == null)
            {
                result = LastChanceFormatter;
            }
            else
            {
                var exceptionType = exception.GetType();

                while (result == null)
                {
                    // convert to dictionary
                    result = ExceptionFormatters.FirstOrDefault(ef => ef.SupportedException == exceptionType);
                    if (result == null)
                    {
                        if (exceptionType.BaseType == null)
                        {
                            return LastChanceFormatter;
                        }

                        exceptionType = exceptionType.BaseType;
                    }
                }
            }

            return result;
        }

        private static void BuildLastChanceMessage(Exception exception, StringBuilder messageBuilder)
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

        private static void BuildExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            messageBuilder.Append("Exception:");
            messageBuilder.Append(exception.GetType().Name);
            messageBuilder.Append(" Message:");
            messageBuilder.Append(exception.Message);
        }

        private static void BuildAggregateExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            var exceptions = ((AggregateException) exception).Flatten();

            messageBuilder.AppendLine($"Aggregate exception has {exceptions.InnerExceptions.Count} inner exception.");
            for (int i = 0; i < exceptions.InnerExceptions.Count; i++)
            {
                messageBuilder.Append("Exception:");
                messageBuilder.Append(i);
                BuildDetailedExceptionMessage(exceptions.InnerExceptions[i], messageBuilder);
            }
        }

        private static void BuildHttpExceptionMessage(Exception exception, StringBuilder messageBuilder)
        {
            var ex = (HttpException) exception;
            messageBuilder.Append("Exception: ");
            messageBuilder.Append(ex.GetType().Name);
            messageBuilder.Append(" Message: ");
            messageBuilder.Append(ex.Message);
            messageBuilder.Append(" HTTP-StatusCode:");
            messageBuilder.Append(ex.GetHttpCode());
            messageBuilder.Append(" HTTP-Message:");
            messageBuilder.Append(ex.GetHtmlErrorMessage());
        }  
    }
}