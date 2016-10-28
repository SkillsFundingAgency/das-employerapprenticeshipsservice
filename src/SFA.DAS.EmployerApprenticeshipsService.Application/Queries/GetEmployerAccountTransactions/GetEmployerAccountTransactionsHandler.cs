using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsHandler : IAsyncRequestHandler<GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private readonly IAggregationRepository _aggregationRepository;
        public GetEmployerAccountTransactionsHandler(IAggregationRepository aggregationRepository)
        {
            _aggregationRepository = aggregationRepository;
        }

        public async Task<GetEmployerAccountTransactionsResponse> Handle(GetEmployerAccountTransactionsQuery message)
        {
            var response = await _aggregationRepository.GetByAccountId(message.AccountId);
            return new GetEmployerAccountTransactionsResponse {Data = response};
        }
    }
}
