﻿using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class LegalEntityRepository : BaseRepository, ILegalEntityRepository
    {
        public LegalEntityRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<LegalEntityView> GetLegalEntityById(long accontId, long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AccountId", accontId, DbType.Int64);
                parameters.Add("@Id", id, DbType.Int64);

                return await c.QueryAsync<LegalEntityView>(
                    sql: "[employer_account].[GetLegalEntity_ById]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
            return result.SingleOrDefault();
        }
    }
}