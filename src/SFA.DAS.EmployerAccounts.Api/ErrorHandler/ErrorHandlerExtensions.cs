using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Api.ErrorHandler;

public static class ErrorHandlerExtensions
{
    public static IApplicationBuilder UseApiGlobalExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        Task Handler(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature != null)
            {
                logger.LogError($"Something went wrong: {contextFeature.Error}");
            }

            return Task.CompletedTask;
        }

        app.UseExceptionHandler(appError =>
        {
            appError.Run(Handler);
        });

        return app;
    }
}