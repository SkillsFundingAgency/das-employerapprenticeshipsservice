using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.CommitmentsV2.Api.Types.Http;

namespace SFA.DAS.EmployerAccounts.Api.Http;

public static class HttpResponseExtensions
{
    public static void SetStatusCode(this HttpResponse httpResponse, HttpStatusCode httpStatusCode)
    {
        httpResponse.StatusCode = (int)httpStatusCode;
    }
        
    public static void SetSubStatusCode(this HttpResponse httpResponse, HttpSubStatusCode httpSubStatusCode)
    {
        httpResponse.Headers[HttpHeaderNames.SubStatusCode] = ((int)httpSubStatusCode).ToString();
    }

    public static Task WriteJsonAsync(this HttpResponse httpResponse, object body)
    {
        httpResponse.ContentType = "application/json";

        return httpResponse.WriteAsync(JsonConvert.SerializeObject(body, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            Formatting = Formatting.Indented
        }));
    }
}