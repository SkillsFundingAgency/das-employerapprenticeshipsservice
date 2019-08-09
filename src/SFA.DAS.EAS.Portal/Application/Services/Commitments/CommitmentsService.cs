using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments
{
    public class CommitmentsService : ICommitmentsService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;

        public CommitmentsService(ICommitmentsApiHttpClientFactory commitmentsApiHttpClientFactory, ILog log)
        {
            _log = log;
            _httpClient = commitmentsApiHttpClientFactory.CreateHttpClient();
        }

        public Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId, 
                CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting commitment {commitmentId} for provider {providerId}");
            //      return await this._commitmentHelper.GetCommitment(string.Format("{0}api/provider/{1}/commitments/{2}", (object) this._configuration.BaseUrl, (object) providerId, (object) commitmentId));

            throw new NotImplementedException();
        }
    }
}