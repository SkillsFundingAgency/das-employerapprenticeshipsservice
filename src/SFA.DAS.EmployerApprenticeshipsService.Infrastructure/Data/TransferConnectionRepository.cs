using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferConnectionRepository : BaseRepository, ITransferConnectionRepository
    {
        public TransferConnectionRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<long> Create(TransferConnection transferConnection)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@senderUserId", transferConnection.SenderUserId, DbType.Int64);
                parameters.Add("@senderAccountId", transferConnection.SenderAccountId, DbType.Int64);
                parameters.Add("@receiverAccountId", transferConnection.ReceiverAccountId, DbType.Int64);
                parameters.Add("@status", transferConnection.Status, DbType.Int16);
                parameters.Add("@createdDate", DateTime.UtcNow, DbType.DateTime);
                parameters.Add("@transferConnectionId", null, DbType.Int64, ParameterDirection.Output);

                await c.ExecuteAsync(
                    sql: "[employer_account].[CreateTransferConnection]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<long>("@transferConnectionId");
            });
        }

        public async Task<TransferConnection> GetCreatedTransferConnection(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@id", id, DbType.Int64);
                parameters.Add("@status", TransferConnectionStatus.Created, DbType.Int16);

                return await c.QueryAsync<TransferConnection>(
                    sql: "SELECT * FROM [employer_account].[TransferConnection] WHERE Id = @id AND Status = @status;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<TransferConnection> GetSentTransferConnection(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@id", id, DbType.Int64);
                parameters.Add("@status", TransferConnectionStatus.Sent, DbType.Int16);

                return await c.QueryAsync<TransferConnection>(
                    sql: "SELECT * FROM [employer_account].[TransferConnection] WHERE Id = @id AND Status = @status;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }


        public async Task Send(TransferConnection transferConnection)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@id", transferConnection.Id, DbType.Int64);
                parameters.Add("@oldStatus", TransferConnectionStatus.Created, DbType.Int16);
                parameters.Add("@newStatus", transferConnection.Status, DbType.Int16);
                parameters.Add("@modifiedDate", DateTime.UtcNow, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "UPDATE [employer_account].[TransferConnection] SET Status = @newStatus, ModifiedDate = @modifiedDate WHERE Id = @id AND Status = @oldStatus;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}