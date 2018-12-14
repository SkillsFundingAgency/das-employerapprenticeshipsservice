using System;
using System.Linq;
using System.Net;
using SFA.DAS.Validation;
using ValidationException = FluentValidation.ValidationException;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     Extension methods to help build up the requirements to a call. A fluent syntax is used allowing the 
    ///     builder methods to be chained together.
    /// </summary>
    public static class CallRequirementsExtensions
    {
        /// <summary>
        ///     Set the call request to expect a <see cref="HttpStatusCode.BadRequest"/> and to ignore
        ///     the <see cref="ValidationException"/> and <see cref="InvalidRequestException"/>.
        /// </summary>
        public static CallRequirements ExpectValidationError(this CallRequirements call)
        {
            return call
                .ExpectStatusCodes(HttpStatusCode.BadRequest)
                .IgnoreExceptionTypes(typeof(InvalidRequestException), typeof(ValidationException));
        }

        /// <summary>
        ///     Specifies the unhandled exception types that, should they occur, will not be treated as 
        ///     an error. Any other unhandled exceptions will cause the call to be considered a failure.
        ///     Note that the test uses a "is", so child exception types will also be accepted.
        /// </summary>
        /// <param name="call"></param>
        /// <param name="exceptionTypes"></param>
        /// <returns></returns>
        public static CallRequirements IgnoreExceptionTypes(this CallRequirements call, params Type[] exceptionTypes)
        {
            call.IgnoreExceptionTypes = exceptionTypes.ToArray();
            return call;
        }

        /// <summary>
        ///     Specifies the response codes that indicate a successful call. Any other status codes will
        ///     cause the call to be considered a failure.
        /// </summary>
        public static CallRequirements ExpectStatusCodes(this CallRequirements call, params HttpStatusCode[] statuscodes)
        {
            call.AcceptableStatusCodes = statuscodes.ToArray();
            return call;
        }

        /// <summary>
        ///     Specify the controller class that is expected to be used in the call. If this class
        ///     is not constructed (from DI) then the call will be considered a failure.
        /// </summary>
        public static CallRequirements ExpectControllerType(this CallRequirements call, Type controllerType)
        {
            call.ExpectedControllerType = controllerType;
            return call;
        }
    }
}