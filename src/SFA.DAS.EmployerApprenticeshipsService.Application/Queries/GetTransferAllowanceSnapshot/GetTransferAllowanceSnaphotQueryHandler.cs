using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowanceSnapshot
{
    public class GetTransferAllowanceSnapshotQueryHandler : IAsyncRequestHandler<GetTransferAllowanceSnapshotQuery, GetTransferAllowanceSnapshotResponse>
    {
        private readonly EmployerFinancialDbContext _db;
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public GetTransferAllowanceSnapshotQueryHandler(EmployerFinancialDbContext db, LevyDeclarationProviderConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<GetTransferAllowanceSnapshotResponse> Handle(GetTransferAllowanceSnapshotQuery message)
        {

            var transferAllowance = await _db.AccountTransferSnapshots
                .Where(snapshot => snapshot.AccountId == message.AccountId && snapshot.Year == message.Year)
                .Select(snapshot => snapshot.TransferAllowance)
                .FirstOrDefaultAsync();

            return new GetTransferAllowanceSnapshotResponse
            {
                TransferAllowance = transferAllowance
            };
        }
    }
}