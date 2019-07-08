using System;

namespace SFA.DAS.EAS.Portal.Client.Exceptions
{
    //todo: use existing, or custom
    // use https://github.com/SkillsFundingAgency/das-provider-relationships/blob/master/src/SFA.DAS.ProviderRelationships.Api.Client/Http/RestHttpClientException.cs
    // & move to http folder (check for any changes in shared)
    public class HttpException : Exception
    {
        public HttpException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
        public HttpException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
