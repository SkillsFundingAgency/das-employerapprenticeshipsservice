using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferRequests;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferRequestRepository : ITransferRequestRepository
    {
        private readonly EmployerAccountDbContext _db;

        public TransferRequestRepository(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public Task Add(TransferRequest transferRequest)
        {
            _db.TransferRequests.Add(transferRequest);

            return _db.SaveChangesAsync();
        }

        public Task<TransferRequest> GetTransferRequestByCommitmentId(long commitmentId)
        {
            return _db.TransferRequests.SingleOrDefaultAsync(r => r.CommitmentId == commitmentId);
        }
    }
}