using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferConnectionInvitationRepository : BaseRepository, ITransferConnectionInvitationRepository
    {
        public TransferConnectionInvitationRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<long> Add(TransferConnectionInvitation transferConnectionInvitation)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@senderUserId", transferConnectionInvitation.SenderUserId, DbType.Int64);
                parameters.Add("@senderAccountId", transferConnectionInvitation.SenderAccountId, DbType.Int64);
                parameters.Add("@receiverAccountId", transferConnectionInvitation.ReceiverAccountId, DbType.Int64);
                parameters.Add("@status", transferConnectionInvitation.Status, DbType.Int16);
                parameters.Add("@createdDate", DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@transferConnectionInvitationId", null, DbType.Int64, ParameterDirection.Output);

                await c.ExecuteAsync(
                    sql: "[employer_account].[CreateTransferConnectionInvitation]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<long>("@transferConnectionInvitationId");
            });
        }

        public async Task<TransferConnectionInvitation> GetSentTransferConnectionInvitation(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@id", id, DbType.Int64);
                parameters.Add("@status", TransferConnectionInvitationStatus.Pending, DbType.Int16);

                const string sql = @"
                    SELECT 
                        i.Id,
                        i.SenderUserId,
                        i.SenderAccountId,
                        i.ReceiverAccountId,
                        i.Status,
                        i.CreatedDate,
                        i.ModifiedDate,
                        r.name AS ReceiverAccountName
                    FROM [employer_account].[TransferConnectionInvitation] i
                    INNER JOIN [employer_account].[Account] r ON i.ReceiverAccountId = r.Id
                    WHERE i.Id = @id
                    AND i.Status = @status
                    ORDER BY i.CreatedDate;";

                return await c.QueryAsync<TransferConnectionInvitation>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<IEnumerable<TransferConnectionInvitation>> GetSentTransferConnectionInvitations(long senderAccountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@senderAccountId", senderAccountId, DbType.Int64);
                parameters.Add("@status", TransferConnectionInvitationStatus.Pending, DbType.Int16);

                const string sql = @"
                    SELECT
                        i.Id,
                        i.SenderUserId,
                        i.SenderAccountId,
                        i.ReceiverAccountId,
                        i.Status,
                        i.CreatedDate,
                        i.ModifiedDate,
                        r.Name AS ReceiverAccountName
                    FROM [employer_account].[TransferConnectionInvitation] i
                    INNER JOIN [employer_account].[Account] r ON i.ReceiverAccountId = r.Id
                    WHERE i.SenderAccountId = @senderAccountId
                    AND i.Status = @status
                    ORDER BY i.CreatedDate;";

                return await c.QueryAsync<TransferConnectionInvitation>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result;
        }

        public async Task<IEnumerable<TransferConnectionInvitation>> GetTransferConnectionInvitations(long senderAccountId, long receiverAccountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@senderAccountId", senderAccountId, DbType.Int64);
                parameters.Add("@receiverAccountId", receiverAccountId, DbType.Int64);

                const string sql = @"
                    SELECT
                        Id,
                        SenderUserId,
                        SenderAccountId,
                        ReceiverAccountId,
                        Status,
                        CreatedDate,
                        ModifiedDate,
                    FROM [employer_account].[TransferConnectionInvitation]
                    WHERE SenderAccountId = @senderAccountId
                    AND ReceiverAccountId = @receiverAccountId;";

                return await c.QueryAsync<TransferConnectionInvitation>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result;
        }
    }
}