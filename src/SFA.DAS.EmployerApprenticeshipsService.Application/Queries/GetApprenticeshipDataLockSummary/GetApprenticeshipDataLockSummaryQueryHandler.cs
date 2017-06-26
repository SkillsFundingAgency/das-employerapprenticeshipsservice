using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetDataLockSummaryQueryHandler : IAsyncRequestHandler<GetDataLockSummaryQueryRequest, GetDataLockSummaryQueryResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        public GetDataLockSummaryQueryHandler(IDataLockApi dataLockApi)
        {
            _dataLockApi = dataLockApi;
        }


        public async Task<GetDataLockSummaryQueryResponse> Handle(GetDataLockSummaryQueryRequest command)
        {
            var response = await _dataLockApi.GetDataLockSummary(command.ApprenticeshipId);

            return new GetDataLockSummaryQueryResponse
            {
                DataLockSummary = response
            };
        }
    }
}