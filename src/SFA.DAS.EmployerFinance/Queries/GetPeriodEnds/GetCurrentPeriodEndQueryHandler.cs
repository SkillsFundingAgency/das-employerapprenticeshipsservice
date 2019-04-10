using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Queries.GetPeriodEnds
{
    public class GetPeriodEndQueryHandler : IAsyncRequestHandler<GetPeriodEndsRequest, GetPeriodEndsResponse>
    {
        private readonly IDasLevyRepository _dasLevyRepository;

        public GetPeriodEndQueryHandler(IDasLevyRepository dasLevyRepository)
        {
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetPeriodEndsResponse> Handle(GetPeriodEndsRequest message)
        {
            var response = new GetPeriodEndsResponse();

            var result = await _dasLevyRepository.GetAllPeriodEnds();
            response.CurrentPeriodEnds = result.ToList();

            return response;
        }
    }
}