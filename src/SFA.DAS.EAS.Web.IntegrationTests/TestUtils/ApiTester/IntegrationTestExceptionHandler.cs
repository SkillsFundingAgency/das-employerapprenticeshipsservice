using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     A decorator for the existing global exception handler which will keep a reference to 
    ///     any unhandled exception that occurred.
    /// </summary>
    public class IntegrationTestExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionHandler _wrappedExceptionHandler;

        public IntegrationTestExceptionHandler(IExceptionHandler runtimeExceptionHandler) 
        {
            _wrappedExceptionHandler = runtimeExceptionHandler;
        }

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            Exception = context.Exception;
            return _wrappedExceptionHandler.HandleAsync(context, cancellationToken);
        }

        public Exception Exception { get; private set; }

        public bool IsFaulted => Exception != null;

        public void ClearException()
        {
            Exception = null;
        }
    }
}