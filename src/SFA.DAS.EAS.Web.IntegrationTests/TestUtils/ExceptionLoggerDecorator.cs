using System.Web.Http.ExceptionHandling;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils
{
    public class ExceptionLoggerDecorator 
    {
        public ExceptionLoggerDecorator(IExceptionHandler wrappedExceptionHandler)
        {
            WrappedExceptionHandler = wrappedExceptionHandler;
        }

        protected IExceptionHandler WrappedExceptionHandler { get; }
    }
}