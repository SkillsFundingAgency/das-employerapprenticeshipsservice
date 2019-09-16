using System.Net.Http;

namespace SFA.DAS.EAS.Portal.Client.Services.Recruit.Http
{
    public interface IRecruitApiHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}