using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Queries.GetCurrentPeriodEnd
{
    public class GetCurrentPeriodEndQueryHandler : IAsyncRequestHandler<GetCurrentPeriodEndRequest, GetPeriodEndResponse>
    {
        private readonly IDasLevyRepository _dasLevyRepository;

        public GetCurrentPeriodEndQueryHandler(IDasLevyRepository dasLevyRepository)
        {
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetPeriodEndResponse> Handle(GetCurrentPeriodEndRequest message)
        {
            var response = new GetPeriodEndResponse();

            var result = await _dasLevyRepository.GetLatestPeriodEnd();
            response.CurrentPeriodEnd = result;

            return response;
        }
    }
}