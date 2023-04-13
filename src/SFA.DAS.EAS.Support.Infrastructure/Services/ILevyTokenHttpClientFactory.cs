using HMRC.ESFA.Levy.Api.Client;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public interface ILevyTokenHttpClientFactory
    {
        Task<IApprenticeshipLevyApiClient> GetLevyHttpClient();
    }
}
