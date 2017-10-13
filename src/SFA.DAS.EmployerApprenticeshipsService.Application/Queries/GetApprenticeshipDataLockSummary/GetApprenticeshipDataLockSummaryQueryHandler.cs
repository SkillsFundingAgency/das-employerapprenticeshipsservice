using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetDataLockSummaryQueryHandler : IAsyncRequestHandler<GetDataLockSummaryQueryRequest, GetDataLockSummaryQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;

        public GetDataLockSummaryQueryHandler(IEmployerCommitmentApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        public async Task<GetDataLockSummaryQueryResponse> Handle(GetDataLockSummaryQueryRequest command)
        {
            var response = await _commitmentApi.GetDataLockSummary(command.AccountId, command.ApprenticeshipId);

            return new GetDataLockSummaryQueryResponse
            {
                DataLockSummary = response
            };
        }
    }
}