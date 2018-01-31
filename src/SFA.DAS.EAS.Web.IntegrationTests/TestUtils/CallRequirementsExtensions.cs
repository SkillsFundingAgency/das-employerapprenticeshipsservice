using System;
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
            return call
                .AllowStatusCodes(HttpStatusCode.BadRequest)
                .IgnoreExceptionTypes(typeof(InvalidRequestException), typeof(ValidationException));
        }

        public static CallRequirements IgnoreExceptionTypes(this CallRequirements call, params Type[] exceptionTypes)
        {
            call.IgnoreExceptionTypes = exceptionTypes.ToArray();
            return call;
        }

        public static CallRequirements AllowStatusCodes(this CallRequirements call, params HttpStatusCode[] statuscodes)
        {
            call.AcceptableStatusCodes = statuscodes.ToArray();
            return call;
        }

        public static CallRequirements ExpectControllerType(this CallRequirements call, Type controllerType)
        {
            call.ExpectedControllerType = controllerType;
            return call;
        }

        public static CallRequirements ExpectResultType(this CallRequirements call, Type resultType)
        {
            call.ExpectResponseType = resultType;
            return call;
        }
    }
}