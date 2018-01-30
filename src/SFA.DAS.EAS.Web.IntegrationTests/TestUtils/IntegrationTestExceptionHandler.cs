using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils
{
    public class IntegrationTestExceptionHandler : ExceptionLoggerDecorator, IExceptionHandler
    {
        public IntegrationTestExceptionHandler(IExceptionHandler runtimeExceptionHandler) : base(runtimeExceptionHandler)
        {
            // just call base        
        }

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            Exception = context.Exception;
            return WrappedExceptionHandler.HandleAsync(context, cancellationToken);
        }

        public Exception Exception { get; private set; }

        public bool IsFaulted => Exception != null;

        public void ClearException()
        {
            Exception = null;
        }
    }
}