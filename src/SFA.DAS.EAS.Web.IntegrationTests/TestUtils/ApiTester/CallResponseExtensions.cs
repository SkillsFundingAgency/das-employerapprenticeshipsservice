using System;
using System.Linq;
using System.Net;
using FluentValidation;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using NUnit.Framework;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    public static class CallResponseExtensions
    {
        /// <summary>
        ///     Set the call request to expect a <see cref="HttpStatusCode.BadRequest"/> and to ignore
        ///     the <see cref="ValidationException"/> and <see cref="InvalidRequestException"/>.
        /// </summary>
        public static void ExpectValidationError(this CallResponse callResponse)
        {
            callResponse.ExpectStatusCodes(HttpStatusCode.BadRequest);
            callResponse.CheckUnhandledExceptions(typeof(InvalidRequestException), typeof(FluentValidation.ValidationException));
        }

        /// <summary>
        ///     Specifies the unhandled exception types that, should they occur, will not be treated as 
        ///     an error. Any other unhandled exceptions will cause the call to be considered a failure.
        ///     Note that the test uses a "is", so child exception types will also be accepted.
        /// </summary>
        public static void CheckUnhandledExceptions(this CallResponse callResponse, params Type[] exceptionTypes)
        {
            if (callResponse.UnhandledException != null)
            {
                Assert.IsTrue(exceptionTypes.Contains(callResponse.UnhandledException.GetType()),
                    $"An unexpected unhandled exception occurred in the server during the call:{callResponse.UnhandledException.GetType().Name} - {callResponse.UnhandledException.Message}");
            }
        }

        /// <summary>
        ///     Specify the controller class that is expected to be used in the call. If this class
        ///     is not constructed (from DI) then the call will be considered a failure.
        /// </summary>
        public static void ExpectControllerType(this CallResponse callResponse, Type expectedControllerType)
        {
            Assert.IsTrue(callResponse.CreatedControllerTypes.Contains(expectedControllerType),
                $"The controller {expectedControllerType.Name} was not created by DI. " +
                $"Controllers that were created are: ({string.Join(",", callResponse.CreatedControllerTypes.Select(ct => ct.Name))})");
        }

        /// <summary>
        ///     Specifies the response codes that indicate a successful call. Any other status codes will
        ///     cause the call to be considered a failure.
        /// </summary>
        public static void ExpectStatusCodes(this CallResponse callResponse, params HttpStatusCode[] statuscodes)
        {
            Assert.IsTrue(statuscodes.Contains(callResponse.Response.StatusCode), $"Received response {callResponse.Response.StatusCode} " +
                                                                                  $"when expected any of [{string.Join(",", statuscodes.Select(sc => sc))}]. " +
                                                                                  $"Additional information sent to the client: {callResponse.Response.ReasonPhrase}. ");
        }
    }
}