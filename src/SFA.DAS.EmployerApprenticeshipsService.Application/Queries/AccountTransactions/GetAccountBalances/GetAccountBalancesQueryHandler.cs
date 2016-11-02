using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances
{
    public class GetAccountBalancesQueryHandler : IAsyncRequestHandler<GetAccountBalancesRequest, GetAccountBalancesResponse>
    {
        private readonly IDasLevyRepository _dasLevyRepository;

        public GetAccountBalancesQueryHandler(IDasLevyRepository dasLevyRepository)
        {
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetAccountBalancesResponse> Handle(GetAccountBalancesRequest message)
        {
            var result = await _dasLevyRepository.GetAccountBalances();

            return new GetAccountBalancesResponse {Accounts = result};
        }
    }
}