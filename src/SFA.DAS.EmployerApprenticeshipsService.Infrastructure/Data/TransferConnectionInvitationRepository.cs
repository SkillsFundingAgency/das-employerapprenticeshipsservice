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

        public Task Add(TransferConnectionInvitation transferConnectionInvitation)
        {
            _db.TransferConnectionInvitations.Add(transferConnectionInvitation);

            return _db.SaveChangesAsync();
        }

        public Task<TransferConnectionInvitation> GetTransferConnectionInvitationById(int transferConnectionInvitationId)
        {
            return _db.TransferConnectionInvitations
                .Include(i => i.Changes)
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Where(i => i.Id == transferConnectionInvitationId)
                .SingleAsync();
        }

        public async Task<TransferConnectionInvitation> GetLatestOutstandingTransferConnectionInvitation(long receiverAccountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@receiverAccountId", receiverAccountId, DbType.Int64);

                return await c.QueryFirstOrDefaultAsync<TransferConnectionInvitation>(
                    sql: "SELECT TOP 1 * "+
                         "FROM [employer_account].[TransferConnectionInvitation] "+
                         $"WHERE ReceiverAccountId = @receiverAccountId AND Status = {(int)TransferConnectionInvitationStatus.Pending} "+
                         "ORDER BY CreatedDate DESC;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result;
        }
    }
}