using System;
using System.Net;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     Specifies the expectations for a call, for example the acceptable status codes/
    /// </summary>
    public class CallRequirements
    {
        private static readonly TimeSpan DefaultTimeOut = new TimeSpan(0,5,0);

        public CallRequirements(string uri)
        {
            Uri = uri;
            TimeOut = DefaultTimeOut;
        }

        /// <summary>
        ///     The URI that will be used for the call. This should not include the hodt name (which will be 
        ///     set automatically).
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        ///     The status codes that will be required to consider the call successful. 
        /// </summary>
        public HttpStatusCode[] AcceptableStatusCodes { get; set; }

        /// <summary>
        ///     Optional. If set then the integration test will verify that an instance of this controller
        ///     was created during the test.
        /// </summary>
        public Type ExpectedControllerType { get; set; }

        /// <summary>
        ///     Ignore these exception types if they occur as an unhandled exception in the server. Unhandled
        ///     exceptions that occur in the server that are not in this list will cause the test to fail.
        /// </summary>
        public Type[] IgnoreExceptionTypes { get; set; } 

        /// <summary>
        ///     HTTP request timeout to use. 
        /// </summary>
        public TimeSpan TimeOut { get; set; }
    }
}