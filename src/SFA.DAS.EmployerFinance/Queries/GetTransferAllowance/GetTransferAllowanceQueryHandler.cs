using MediatR;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQueryHandler : IAsyncRequestHandler<GetTransferAllowanceQuery, GetTransferAllowanceResponse>
    {
        private readonly EmployerFinanceDbContext _db;
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public GetTransferAllowanceQueryHandler(EmployerFinanceDbContext db, LevyDeclarationProviderConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<GetTransferAllowanceResponse> Handle(GetTransferAllowanceQuery message)
        {
            var transferAllowance = await _db.GetTransferAllowance(message.AccountId.Value, _configuration.TransferAllowancePercentage);

            return new GetTransferAllowanceResponse
            {
                TransferAllowance = transferAllowance
            };
        }
    }
}