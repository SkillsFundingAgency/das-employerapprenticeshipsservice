using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils
{
    /// <summary>
    ///     Specifies the expectations for a call, for example the acceptable status codes/
    /// </summary>
    public class CallRequirements
    {
        public CallRequirements(string uri, HttpStatusCode acceptableStatusCode) : this(uri, new[] { acceptableStatusCode })
        {
        }

        public CallRequirements(string uri, IEnumerable<HttpStatusCode> acceptableStatusCodes)
        {
            Uri = uri;
            AcceptableStatusCodes = acceptableStatusCodes.ToArray();
        }

        /// <summary>
        ///     The URI that will be used for the call. This should not include the hodt name (which will be 
        ///     set automatically).
        /// </summary>
        public string Uri { get; }

        /// <summary>
        ///     The status codes that will be required to consider the call successful. 
        /// </summary>
        /// <remarks>
        ///     Note that if the Controller throws an unhandled exception (i.e. an exception is caught be the 
        ///     global exception handler) then the call will be considered a fail regardless of the 
        ///     status code returned.
        /// </remarks>
        public HttpStatusCode[] AcceptableStatusCodes { get; }

        /// <summary>
        ///     Optional. If set then the integration test will verify that an instance of this controller
        ///     was created during the test.
        /// </summary>
        public Type ExpectedControllerType { get; set; }
    }

    /// <summary>
    ///     A generic form of <see cref="CallRequirements"/> which will deserialise the full response into the
    ///     the specified type.
    /// </summary>
    public class CallRequirements<TResultType> : CallRequirements
    {
        public CallRequirements(string uri, HttpStatusCode acceptableStatusCode) : this(uri, new[] { acceptableStatusCode })
        {
            // Just call other constructor.
        }

        public CallRequirements(string uri, IEnumerable<HttpStatusCode> acceptableStatusCodes) : base(uri, acceptableStatusCodes)
        {
            // just call base
        }

        private TResultType Result { get; set; }
    }

}