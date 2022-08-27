using MediatR;
using SFA.DAS.EmployerFinance.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQueryHandler : IAsyncRequestHandler<GetTransferAllowanceQuery, GetTransferAllowanceResponse>
    {
        private readonly IDasLevyRepository _dasLevyRepository;

        public GetTransferAllowanceQueryHandler(IDasLevyRepository dasLevyRepository)
        {
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetTransferAllowanceResponse> Handle(GetTransferAllowanceQuery message)
        {
            var transferAllowance = await _dasLevyRepository.GetTransferAllowance(message.AccountId.Value);

            return new GetTransferAllowanceResponse
            {
                TransferAllowance = transferAllowance
            };
        }
    }   
}
