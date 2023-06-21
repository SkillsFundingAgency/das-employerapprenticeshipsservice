using HMRC.ESFA.Levy.Api.Client;

namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface ILevyTokenHttpClientFactory
{
    Task<IApprenticeshipLevyApiClient> GetLevyHttpClient();
}
