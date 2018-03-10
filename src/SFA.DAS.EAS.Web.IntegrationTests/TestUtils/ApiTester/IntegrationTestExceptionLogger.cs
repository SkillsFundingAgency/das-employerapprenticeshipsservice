using System;
using System.Web.Http.ExceptionHandling;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     An exception logger which will keep a reference to any unhandled exception that occurred.
    /// </summary>
    public class IntegrationTestExceptionLogger : ExceptionLogger
    {
        public Exception Exception { get; private set; }
        public bool IsFaulted => Exception != null;

        public override void Log(ExceptionLoggerContext context)
        {
            Exception = context.Exception;
        }

        public void ClearException()
        {
            Exception = null;
        }
    }
}