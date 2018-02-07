using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferConnectionInvitationRepository : ITransferConnectionInvitationRepository
    {
        private readonly EmployerAccountDbContext _db;

        public TransferConnectionInvitationRepository(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task Add(TransferConnectionInvitation transferConnectionInvitation)
        {
            _db.TransferConnectionInvitations.Add(transferConnectionInvitation);

            await _db.SaveChangesAsync();
        }

        public async Task<TransferConnectionInvitation> GetTransferConnectionInvitationToApproveOrReject(long transferConnectionInvitationId, long receiverAccountId)
        {
            return await _db.TransferConnectionInvitations
                .Include(i => i.SenderAccount)
                .Include(i => i.ReceiverAccount)
                .Where(i => 
                    i.Id == transferConnectionInvitationId &&
                    i.ReceiverAccount.Id == receiverAccountId &&
                    i.Status == TransferConnectionInvitationStatus.Pending
                )
                .SingleAsync();
        }
    }
}