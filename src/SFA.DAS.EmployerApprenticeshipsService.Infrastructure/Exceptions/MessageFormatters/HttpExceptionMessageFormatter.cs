using SFA.DAS.EAS.Infrastructure.Extensions;
using System;
using System.Text;
using System.Web;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class HttpExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(HttpException);

        protected override void CreateFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var httpException = (HttpException)exception;

            messageBuilder.AppendLine($"Exception: {httpException.GetType().Name}");
            messageBuilder.AppendLine($"Message: {httpException.Message}");
            messageBuilder.AppendLine($"HTTP-StatusCode: {httpException.GetHttpCode()}");
            messageBuilder.Append($"HTTP-Message: {httpException.GetHtmlErrorMessage()}");

            exception.InnerException?.AppendMessage(messageBuilder);
        }
    }
}
