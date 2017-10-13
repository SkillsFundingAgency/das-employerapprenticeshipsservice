using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetPriceHistoryQueryRequest
{
    public class GetPriceHistoryQueryHandler :
        IAsyncRequestHandler<GetPriceHistoryQueryRequest, GetPriceHistoryQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;

        public GetPriceHistoryQueryHandler(IEmployerCommitmentApi commitmentApi)
        {
            _commitmentApi = commitmentApi;
        }

        public async Task<GetPriceHistoryQueryResponse> Handle(GetPriceHistoryQueryRequest message)
        {
            var response = await _commitmentApi.GetPriceHistory(message.AccountId, message.ApprenticeshipId);

            return new GetPriceHistoryQueryResponse
            {
                History = response.ToList()
            };
        }
    }
}