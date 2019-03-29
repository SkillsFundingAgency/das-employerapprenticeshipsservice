using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQueryHandler : IAsyncRequestHandler<GetTransferAllowanceQuery, GetTransferAllowanceResponse>
    {
        private readonly EmployerFinanceDbContext _db;
        private readonly EmployerFinanceConfiguration _configuration;

        public GetTransferAllowanceQueryHandler(EmployerFinanceDbContext db, EmployerFinanceConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<GetTransferAllowanceResponse> Handle(GetTransferAllowanceQuery message)
        {
            var transferAllowance = await _db.GetTransferAllowance(message.AccountId.Value, _configuration.TransferAllowancePercentage);

            return new GetTransferAllowanceResponse
            {
                TransferAllowance = transferAllowance,
                TransferAllowancePercentage = _configuration.TransferAllowancePercentage
            };
        }
    }
}