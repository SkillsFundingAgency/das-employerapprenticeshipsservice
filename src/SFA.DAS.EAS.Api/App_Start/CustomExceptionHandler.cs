using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using FluentValidation;
using NLog;

namespace SFA.DAS.EAS.Api
{
    public class CustomExceptionHandler : ExceptionHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is ValidationException)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                var message = ((ValidationException)context.Exception).Message;
                response.Content = new StringContent(message);
                context.Result = new ValidationErrorResult(context.Request, response);
                return;
            }

            Logger.Error(context.Exception, "Unhandled exception - " + context.Exception.Message);
            base.Handle(context);
        }
    }
}