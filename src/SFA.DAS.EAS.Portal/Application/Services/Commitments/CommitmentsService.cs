using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments
{
    public class CommitmentsService : ICommitmentsService
    {
        private readonly ILog _log;
        private readonly RestHttpClient _restHttpClient;

        public CommitmentsService(ICommitmentsApiHttpClientFactory commitmentsApiHttpClientFactory, ILog log)
        {
            _log = log;
            _restHttpClient = new RestHttpClient(commitmentsApiHttpClientFactory.CreateHttpClient());
        }

        public Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId, 
                CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting commitment {commitmentId} for provider {providerId}");

            return _restHttpClient.Get<CommitmentView>("/api/provider/{providerId}/commitments/{commitmentId}", null,
                cancellationToken);
        }
    }
}