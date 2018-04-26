using System;
using System.Linq;
using System.Text;

namespace SFA.DAS.EAS.Application.Extensions
{
    public static class ExceptionExtensions
    {
        #region original method

        public static string GetMessage(this Exception ex)
        {
            var newlines = Environment.NewLine.ToArray();
            var messageBuilder = new StringBuilder(ex.Message.Trim(newlines));

            while (ex.InnerException != null)
            {
                messageBuilder.AppendLine();
                messageBuilder.Append(ex.InnerException.Message.Trim(newlines));

                ex = ex.InnerException;
            }

            return messageBuilder.ToString();
        }

        #endregion Begin region         

        #region new code

        private class ExceptionTypeFormatter
        {
            public ExceptionTypeFormatter(Type supportedException, Action<Exception, StringBuilder> builder)
            {
                SupportedException = supportedException;
                Build = builder;
            }

            public Type SupportedException { get;  }
            public Action<Exception, StringBuilder> Build { get; }
        }

        private static readonly ExceptionTypeFormatter LastChanceFormatter = new ExceptionTypeFormatter(typeof(Exception), BuildLastChanceMessage);

        private static readonly ExceptionTypeFormatter[] ExceptionFormatters = new[]
        {
            // This will catch all exceptions
            new ExceptionTypeFormatter(typeof(Exception), BuildExceptionMessage),
            new ExceptionTypeFormatter(typeof(AggregateException), BuildAggregateExceptionMessage)
        };


        public static string GetDetailedExceptionMessage(Exception exception)
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

            var handler = GetAppropriateExceptionFormatter(exception);
            handler.Build(exception, messageBuilder);

            BuildAggregateExceptionMessage(exception.InnerException, messageBuilder);
        }

        private static ExceptionTypeFormatter GetAppropriateExceptionFormatter(Exception exception)
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
                    result = ExceptionFormatters.First(ef => ef.SupportedException == exceptionType);
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
                messageBuilder.Append("exception ");
                messageBuilder.Append(i);
                messageBuilder.Append(':');
                BuildDetailedExceptionMessage(exceptions.InnerExceptions[i], messageBuilder);
            }
        }

        #endregion Begin region         
    }
}