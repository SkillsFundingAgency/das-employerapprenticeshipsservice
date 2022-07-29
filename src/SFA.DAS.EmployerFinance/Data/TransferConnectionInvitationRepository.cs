using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Data
{
    public class TransferConnectionInvitationRepository : ITransferConnectionInvitationRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public TransferConnectionInvitationRepository(Lazy<EmployerFinanceDbContext> db)
        {
            _db = db;
        }

        public Task Add(TransferConnectionInvitation transferConnectionInvitation)
        {
            _db.Value.TransferConnectionInvitations.Add(transferConnectionInvitation);

            // TO DO: investigate why this is saved here - shouldn't the unit of work pattern be saving it with the published events
            return _db.Value.SaveChangesAsync();
        }

        public Task<TransferConnectionInvitation> Get(int id)
        {
            return _db.Value.TransferConnectionInvitations
                .Include(i => i.Changes)
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<TransferConnectionInvitation> GetBySender(int id, long senderAccountId, TransferConnectionInvitationStatus status)
        {
            return await _db.Value.TransferConnectionInvitations
                .Where(i =>
                    i.Id == id &&
                    i.SenderAccount.Id == senderAccountId &&
                    i.Status == status)
                .SingleOrDefaultAsync();
        }

        public async Task<TransferConnectionInvitation> GetByReceiver(int id, long receiverAccountId, TransferConnectionInvitationStatus status)
        {
            return await _db.Value.TransferConnectionInvitations
                .Where(i =>
                    i.Id == id &&
                    i.ReceiverAccount.Id == receiverAccountId &&
                    i.Status == status)
                .SingleOrDefaultAsync();
        }

        public async Task<List<TransferConnectionInvitation>> GetByReceiver(long recieverAccountId, TransferConnectionInvitationStatus status)
        {
            return await _db.Value.TransferConnectionInvitations
                .Where(
                    i => i.ReceiverAccount.Id == recieverAccountId && 
                    i.Status == TransferConnectionInvitationStatus.Approved)
                .OrderBy(i => i.SenderAccount.Name)
                .ToListAsync();
        }

        public async Task<TransferConnectionInvitation> GetLatestByReceiver(long receiverAccountId, TransferConnectionInvitationStatus status)
        {
            return await _db.Value.TransferConnectionInvitations
                .Where(
                    i => i.ReceiverAccountId == receiverAccountId && 
                    i.Status ==  status)
                .OrderByDescending(i => i.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<TransferConnectionInvitation> GetBySenderOrReceiver(int id, long accountId)
        {
            return await _db.Value.TransferConnectionInvitations
                .Where(i =>
                    i.Id == id && (
                    i.SenderAccount.Id == accountId && !i.DeletedBySender ||
                    i.ReceiverAccount.Id == accountId))
                .SingleOrDefaultAsync();
        }

        public async Task<List<TransferConnectionInvitation>> GetBySenderOrReceiver(long accountId)
        {
            return await _db.Value.TransferConnectionInvitations
                .Where(
                    i => i.SenderAccount.Id == accountId && !i.DeletedBySender || 
                    i.ReceiverAccount.Id == accountId && !i.DeletedByReceiver)
                .OrderBy(i => i.ReceiverAccount.Id == accountId ? i.SenderAccount.Name : i.ReceiverAccount.Name)
                .ThenBy(i => i.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> AnyTransferConnectionInvitations(long senderAccountId, long receiverAccountId, List<TransferConnectionInvitationStatus> statuses)
        {
            return await _db.Value.TransferConnectionInvitations.AnyAsync(i =>
                i.SenderAccount.Id == senderAccountId && 
                i.ReceiverAccount.Id == receiverAccountId && 
                statuses.Contains(i.Status));
        }
    }
}