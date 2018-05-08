using System;
using System.Text;
using System.Web;

namespace SFA.DAS.EAS.Infrastructure.Exceptions.MessageFormatters
{
    internal class HttpExceptionMessageFormatter : BaseExceptionMessageFormatter
    {
        public override Type SupportedException => typeof(HttpException);

        public override void AppendFormattedMessage(Exception exception, StringBuilder messageBuilder)
        {
            var httpException = exception as HttpException;

            messageBuilder.Append("Exception: ");
            messageBuilder.Append(httpException.GetType().Name);
            messageBuilder.Append(" Message: ");
            messageBuilder.Append(httpException.Message);
            messageBuilder.Append(" HTTP-StatusCode:");
            messageBuilder.Append(httpException.GetHttpCode());
            messageBuilder.Append(" HTTP-Message:");
            messageBuilder.Append(httpException.GetHtmlErrorMessage());
        }
    }
}
