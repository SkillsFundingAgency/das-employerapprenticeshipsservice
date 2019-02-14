using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class InvitationRepository : BaseRepository, IInvitationRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public InvitationRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<List<InvitationView>> Get(string userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userId", Guid.Parse(userId), DbType.Guid);

            var result = await _db.Value.Database.Connection.QueryAsync<InvitationView>(
                sql: "SELECT * FROM [employer_account].[GetInvitations] WHERE ExternalUserId = @userId AND Status = 1;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.ToList();
        }

        public async Task<InvitationView> GetView(long id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", id, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<InvitationView>(
                sql: "SELECT * FROM [employer_account].[GetInvitations] WHERE Id = @id;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<long> Create(Invitation invitation)
        {
            var invitationId = 0L;
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", invitation.AccountId, DbType.Int64);
            parameters.Add("@name", invitation.Name, DbType.String);
            parameters.Add("@email", invitation.Email, DbType.String);
            parameters.Add("@expiryDate", invitation.ExpiryDate, DbType.DateTime);
            parameters.Add("@statusId", invitation.Status, DbType.Int16);
            parameters.Add("@role", invitation.Role, DbType.Int16);
            parameters.Add("@invitationId", invitationId, DbType.Int64,ParameterDirection.Output);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[CreateInvitation]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<long>("@invitationId");
        }

        public async Task<Invitation> Get(long id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", id, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<Invitation>(
                sql: "SELECT * FROM [employer_account].[Invitation] WHERE Id = @id;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<Invitation> Get(long accountId, string email)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@email", email, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<Invitation>(
                sql: "SELECT * FROM [employer_account].[Invitation] WHERE AccountId = @accountId AND Email = @email;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public Task ChangeStatus(Invitation invitation)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", invitation.Id, DbType.Int64);
            parameters.Add("@statusId", invitation.Status, DbType.Int16);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "UPDATE [employer_account].[Invitation] SET Status = CASE WHEN @statusId = 1 AND ExpiryDate < GETDATE() THEN 3 ELSE @statusId END WHERE Id = @id;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public Task Resend(Invitation invitation)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", invitation.Id, DbType.Int64);
            parameters.Add("@name", invitation.Name, DbType.String);
            parameters.Add("@role", invitation.Role, DbType.Int16);
            parameters.Add("@statusId", invitation.Status, DbType.Int16);
            parameters.Add("@expiryDate", invitation.ExpiryDate, DbType.DateTime);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "UPDATE [employer_account].[Invitation] SET Name = @name, Role = @role, Status = @statusId, ExpiryDate = @expiryDate WHERE Id = @id;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public Task Accept(string email, long accountId, Role role)
        { 
            var parameters = new DynamicParameters();

            parameters.Add("@email", email, DbType.String);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@role", role, DbType.Int16);
                
            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[AcceptInvitation]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetNumberOfInvites(string userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ref", Guid.Parse(userId), DbType.Guid);

            var result = await _db.Value.Database.Connection.QueryAsync<int>(
                sql: "[employer_account].[GetNumberOfInvitations_ByUserRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }
    }
}