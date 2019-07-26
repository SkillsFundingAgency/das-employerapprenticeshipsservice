using System.Net.Http;

namespace SFA.DAS.EAS.Portal.Client.Http
{
    public interface IRecruitApiHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}