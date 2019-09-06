using System.Net.Http;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments.Http
{
    public interface ICommitmentsApiHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}