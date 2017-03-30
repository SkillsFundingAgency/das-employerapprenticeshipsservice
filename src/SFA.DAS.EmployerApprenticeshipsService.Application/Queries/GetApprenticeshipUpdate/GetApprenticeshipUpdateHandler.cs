using System;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipUpdate
{
    public class GetApprenticeshipUpdateHandler : IAsyncRequestHandler<GetApprenticeshipUpdateRequest, GetApprenticeshipUpdateResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;

        public GetApprenticeshipUpdateHandler(IEmployerCommitmentApi commitmentApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));

            _commitmentApi = commitmentApi;
        }

        public async Task<GetApprenticeshipUpdateResponse> Handle(GetApprenticeshipUpdateRequest message)
        {
            var result = await _commitmentApi.GetPendingApprenticeshipUpdate(message.AccountId, message.ApprenticehsipId);
            return new GetApprenticeshipUpdateResponse { ApprenticeshipUpdate = result };
        }
    }
}