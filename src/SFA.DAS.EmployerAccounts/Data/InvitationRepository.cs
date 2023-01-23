﻿using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Data;

public class InvitationRepository : BaseRepository, IInvitationRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public InvitationRepository(EmployerAccountsConfiguration configuration, ILogger<InvitationRepository> logger, Lazy<EmployerAccountsDbContext> db)
        : base(configuration.DatabaseConnectionString, logger)
    {
        _db = db;
    }

    public async Task<List<InvitationView>> Get(string userId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@userId", Guid.Parse(userId), DbType.Guid);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<InvitationView>(
            sql: "SELECT * FROM [employer_account].[GetInvitations] WHERE ExternalUserId = @userId AND Status = 1;",
            param: parameters,
            commandType: CommandType.Text);

        return result.ToList();
    }

    public async Task<InvitationView> GetView(long id)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@id", id, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<InvitationView>(
            sql: "SELECT * FROM [employer_account].[GetInvitations] WHERE Id = @id;",
            param: parameters,
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
        parameters.Add("@invitationId", invitationId, DbType.Int64, ParameterDirection.Output);

        await _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[CreateInvitation]",
            param: parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<long>("@invitationId");
    }

    public async Task<Invitation> Get(long id)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@id", id, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<Invitation>(
            sql: "SELECT * FROM [employer_account].[Invitation] WHERE Id = @id;",
            param: parameters,
            commandType: CommandType.Text);

        return result.SingleOrDefault();
    }

    public async Task<Invitation> Get(long accountId, string email)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);
        parameters.Add("@email", email, DbType.String);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<Invitation>(
            sql: "SELECT * FROM [employer_account].[Invitation] WHERE AccountId = @accountId AND Email = @email;",
            param: parameters,
            commandType: CommandType.Text);

        return result.SingleOrDefault();
    }

    public Task ChangeStatus(Invitation invitation)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@id", invitation.Id, DbType.Int64);
        parameters.Add("@statusId", invitation.Status, DbType.Int16);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "UPDATE [employer_account].[Invitation] SET Status = CASE WHEN @statusId = 1 AND ExpiryDate < GETDATE() THEN 3 ELSE @statusId END WHERE Id = @id;",
            param: parameters,
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

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "UPDATE [employer_account].[Invitation] SET Name = @name, Role = @role, Status = @statusId, ExpiryDate = @expiryDate WHERE Id = @id;",
            param: parameters,
            commandType: CommandType.Text);
    }

    public Task Accept(string email, long accountId, Role role)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@email", email, DbType.String);
        parameters.Add("@accountId", accountId, DbType.Int64);
        parameters.Add("@role", role, DbType.Int16);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[AcceptInvitation]",
            param: parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetNumberOfInvites(string userId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@ref", Guid.Parse(userId), DbType.Guid);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<int>(
            sql: "[employer_account].[GetNumberOfInvitations_ByUserRef]",
            param: parameters,
            commandType: CommandType.StoredProcedure);

        return result.SingleOrDefault();
    }
}