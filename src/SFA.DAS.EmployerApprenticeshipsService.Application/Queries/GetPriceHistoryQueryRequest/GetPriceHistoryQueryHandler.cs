using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetPriceHistoryQueryRequest
{
    public class GetPriceHistoryQueryHandler :
        IAsyncRequestHandler<GetPriceHistoryQueryRequest, GetPriceHistoryQueryResponse>
    {
        private readonly IApprenticeshipApi _apprenticeshipApi;

        public GetPriceHistoryQueryHandler(IApprenticeshipApi apprenticeshipApi)
        {
            _apprenticeshipApi = apprenticeshipApi;
        }

        public async Task<GetPriceHistoryQueryResponse> Handle(GetPriceHistoryQueryRequest message)
        {
            var response = await _apprenticeshipApi.GetPriceHistory(message.ApprenticeshipId);

            return new GetPriceHistoryQueryResponse
            {
                History = response.ToList()
            };
        }
    }
}