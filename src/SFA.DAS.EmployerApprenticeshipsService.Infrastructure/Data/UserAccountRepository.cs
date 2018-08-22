﻿using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UserAccountRepository : BaseRepository, IUserAccountRepository
    {
        private readonly Lazy<EmployerAccountDbContext> _db;

        public UserAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<Accounts<Account>> GetAccountsByUserRef(string userRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userRef", Guid.Parse(userRef), DbType.Guid);

            var result = await _db.Value.Database.Connection.QueryAsync<Account>(
                sql: @"[employer_account].[GetAccounts_ByUserRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return new Accounts<Account>
            {
                AccountList = (List<Account>)result
            };
        }

        public async Task<User> Get(string email)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", email, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql: "SELECT Id, CONVERT(NVARCHAR(50), UserRef) AS UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE Email = @email;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<User> Get(long id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@id", id, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql: "SELECT Id, CONVERT(NVARCHAR(50), UserRef) AS UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE Id = @id;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }
    }
}