using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EAS.Portal.Application.Services.Commitments.Http;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments
{
    public class CommitmentsService : ICommitmentsService
    {
        private readonly ILogger<CommitmentsService> _log;
        private readonly RestHttpClient _restHttpClient;

        public CommitmentsService(ICommitmentsApiHttpClientFactory commitmentsApiHttpClientFactory, ILogger<CommitmentsService> log)
        {
            _log = log;
            _restHttpClient = new RestHttpClient(commitmentsApiHttpClientFactory.CreateHttpClient());
        }

        public Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId, 
                CancellationToken cancellationToken = default)
        {
            _log.LogInformation($"Getting commitment {commitmentId} for provider {providerId}");

            return _restHttpClient.Get<CommitmentView>($"/api/provider/{providerId}/commitments/{commitmentId}",
        null, cancellationToken);
        }
    }
}