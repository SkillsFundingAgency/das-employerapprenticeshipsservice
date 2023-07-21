using System.Net.Http.Headers;

namespace SFA.DAS.EmployerAccounts.Interfaces.Hmrc;

public interface IHttpClientWrapper
{
    string BaseUrl { get; set; }
    List<MediaTypeWithQualityHeaderValue> MediaTypeWithQualityHeaderValueList { get; set; }
    string AuthScheme { get; set; }
    Task<string> SendMessage<T>(T content, string url);
    Task<T> Get<T>(string authToken, string url);
    Task<string> GetString(string url, string accessToken);
}