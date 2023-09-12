using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace SFA.DAS.EAS.Application.Http;

[Serializable]
public class RestHttpClientException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ReasonPhrase { get; }
    public Uri RequestUri { get; }
    public string ErrorResponse { get; }

    public RestHttpClientException(HttpResponseMessage httpResponseMessage, string errorResponse)
        : base(GenerateMessage(httpResponseMessage, errorResponse))
    {
        StatusCode = httpResponseMessage.StatusCode;
        ReasonPhrase = httpResponseMessage.ReasonPhrase;
        RequestUri = httpResponseMessage.RequestMessage?.RequestUri;
        ErrorResponse = errorResponse;
    }

    private static string GenerateMessage(HttpResponseMessage httpResponseMessage, string errorResponse)
    {
        return $@"Request '{httpResponseMessage.RequestMessage?.RequestUri}' 
                    returned {(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}
                    Response: {errorResponse}";
    }

    protected RestHttpClientException(SerializationInfo info, StreamingContext context,
        HttpResponseMessage httpResponseMessage, string errorResponse) : base(info, context)
    {
        GenerateMessage(httpResponseMessage, errorResponse);
    }
}