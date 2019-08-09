using System.Net.Http;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments
{
    public interface ICommitmentsApiHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}