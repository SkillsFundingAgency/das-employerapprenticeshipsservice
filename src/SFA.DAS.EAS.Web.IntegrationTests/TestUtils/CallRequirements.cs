using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation;
using SFA.DAS.EAS.Application;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils
{
    public static class CallRequirementsExtensions
    {
        /// <summary>
        ///     Set the call request to expect a <see cref="HttpStatusCode.BadRequest"/> and to ignore
        ///     the <see cref="ValidationException"/> and <see cref="InvalidRequestException"/>.
        /// </summary>
        public static CallRequirements ExpectValidationError(this CallRequirements call)
        {
            call.AcceptableStatusCodes = new[] {HttpStatusCode.BadRequest};
            call.IgnoreExceptionTypes = new[] {typeof(InvalidRequestException), typeof(ValidationException)};
            return call;
        }

        public static CallRequirements<TResult> ExpectValidationError<TResult>(this CallRequirements<TResult> call)
        {
            call.AcceptableStatusCodes = new[] { HttpStatusCode.BadRequest };
            call.IgnoreExceptionTypes = new[] { typeof(InvalidRequestException), typeof(ValidationException) };
            return call;
        }
    }

    /// <summary>
    ///     Specifies the expectations for a call, for example the acceptable status codes/
    /// </summary>
    public class CallRequirements
    {
        // ReSharper disable once StaticMemberInGenericType
        protected static readonly HttpStatusCode[] EmptyStatusCodes = new HttpStatusCode[0];

        private static readonly TimeSpan DefaultTimeOut = new TimeSpan(0,5,0);

        public CallRequirements(string uri) : this(uri, EmptyStatusCodes)
        {
            // just call other constructor
        }

        public CallRequirements(string uri, HttpStatusCode acceptableStatusCode) : this(uri, new[] { acceptableStatusCode })
        {
        }

        public CallRequirements(string uri, IEnumerable<HttpStatusCode> acceptableStatusCodes)
        {
            Uri = uri;
            AcceptableStatusCodes = acceptableStatusCodes.ToArray();
            TimeOut = DefaultTimeOut;
        }

        /// <summary>
        ///     The URI that will be used for the call. This should not include the hodt name (which will be 
        ///     set automatically).
        /// </summary>
        public string Uri { get; }

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

    /// <summary>
    ///     A generic form of <see cref="CallRequirements"/> which will deserialise the full response into the
    ///     the specified type.
    /// </summary>
    public class CallRequirements<TResultType> : CallRequirements
    {
        public CallRequirements(string uri) : this(uri, EmptyStatusCodes)
        {
            // just call other constructor
        }

        public CallRequirements(string uri, HttpStatusCode acceptableStatusCode) : this(uri, new[] { acceptableStatusCode })
        {
            // Just call other constructor.
        }

        public CallRequirements(string uri, IEnumerable<HttpStatusCode> acceptableStatusCodes) : base(uri, acceptableStatusCodes)
        {
            // just call base
        }

        /// <summary>
        ///     The response returned from the call.
        /// </summary>
        public TResultType Result { get; set; }
    }
}