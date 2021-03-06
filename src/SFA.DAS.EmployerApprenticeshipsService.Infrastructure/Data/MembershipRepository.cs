﻿using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class MembershipRepository : BaseRepository, IMembershipRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public MembershipRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@externalUserId", Guid.Parse(externalUserId), DbType.Guid);

            var result = await _db.Value.Database.Connection.QueryAsync<MembershipView>(
                sql: "[employer_account].[GetTeamMember]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }
    }
}