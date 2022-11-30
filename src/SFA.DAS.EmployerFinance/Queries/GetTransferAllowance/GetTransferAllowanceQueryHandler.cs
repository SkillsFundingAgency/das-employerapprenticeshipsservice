using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQueryHandler : IAsyncRequestHandler<GetTransferAllowanceQuery, GetTransferAllowanceResponse>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly EmployerFinanceConfiguration _configuration;

        public GetTransferAllowanceQueryHandler(ITransferRepository transferRepository, EmployerFinanceConfiguration configuration)
        {
            _transferRepository = transferRepository;
            _configuration = configuration;
        }

        public async Task<GetTransferAllowanceResponse> Handle(GetTransferAllowanceQuery message)
        {
            var transferAllowance = await _transferRepository.GetTransferAllowance(message.AccountId, _configuration.TransferAllowancePercentage);

            return new GetTransferAllowanceResponse
            {
                TransferAllowance = transferAllowance,
                TransferAllowancePercentage = _configuration.TransferAllowancePercentage
            };
        }
    }
}