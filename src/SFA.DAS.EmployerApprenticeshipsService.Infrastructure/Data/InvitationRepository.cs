using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class InvitationRepository : BaseRepository, IInvitationRepository
    {
        public InvitationRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
        }

        public async Task<List<InvitationView>> Get(string userId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", Guid.Parse(userId), DbType.Guid);

                return await c.QueryAsync<InvitationView>(
                    sql: "SELECT * FROM [dbo].[GetInvitations] WHERE ExternalUserId = @userId AND Status = 1;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<InvitationView> GetView(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                return await c.QueryAsync<InvitationView>(
                    sql: "SELECT * FROM [dbo].[GetInvitations] WHERE Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task Create(Invitation invitation)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", invitation.AccountId, DbType.Int64);
                parameters.Add("@name", invitation.Name, DbType.String);
                parameters.Add("@email", invitation.Email, DbType.String);
                parameters.Add("@expiryDate", invitation.ExpiryDate, DbType.DateTime);
                parameters.Add("@statusId", invitation.Status, DbType.Int16);
                parameters.Add("@roleId", invitation.RoleId, DbType.Int16);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [dbo].[Invitation] ([AccountId],[Name],[Email],[ExpiryDate],[Status],[RoleId]) VALUES (@accountId, @name, @email, @expiryDate, @statusId, @roleId)",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<Invitation> Get(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                return await c.QueryAsync<Invitation>(
                    sql: "SELECT * FROM [dbo].[Invitation] WHERE Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<Invitation> Get(long accountId, string email)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@email", email, DbType.String);

                return await c.QueryAsync<Invitation>(
                    sql: "SELECT * FROM [dbo].[Invitation] WHERE AccountId = @accountId AND Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task ChangeStatus(Invitation invitation)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", invitation.Id, DbType.Int64);
                parameters.Add("@statusId", invitation.Status, DbType.Int16);

                return await c.ExecuteAsync(
                    sql: "UPDATE [dbo].[Invitation] SET Status = CASE WHEN @statusId = 1 AND ExpiryDate < GETDATE() THEN 3 ELSE @statusId END WHERE Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task Resend(Invitation invitation)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", invitation.Id, DbType.Int64);
                parameters.Add("@name", invitation.Name, DbType.String);
                parameters.Add("@roleId", invitation.RoleId, DbType.Int16);
                parameters.Add("@statusId", invitation.Status, DbType.Int16);
                parameters.Add("@expiryDate", invitation.ExpiryDate, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "UPDATE [dbo].[Invitation] SET Name = @name, RoleId = @roleId, Status = @statusId, ExpiryDate = @expiryDate WHERE Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task Accept(string email, long accountId, short roleId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email, DbType.String);
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@roleId", roleId, DbType.Int16);

                return await c.ExecuteAsync(
                    sql: "[dbo].[AcceptInvitation]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<int> GetNumberOfInvites(string userId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", Guid.Parse(userId), DbType.Guid);

                return await c.QueryAsync<int>(
                    sql: "[dbo].[GetNumberOfInvitations_ByUserId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }
    }
}