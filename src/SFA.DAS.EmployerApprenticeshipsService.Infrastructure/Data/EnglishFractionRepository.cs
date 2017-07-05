﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EnglishFractionRepository : BaseRepository, IEnglishFractionRepository
    {
        public EnglishFractionRepository(LevyDeclarationProviderConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }
        
        public async Task<DateTime> GetLastUpdateDate()
        {
            var result = await WithConnection(async c => await c.QueryAsync<DateTime>(
                sql: "SELECT Top(1) DateCalculated FROM [employer_financial].[EnglishFractionCalculationDate] ORDER BY DateCalculated DESC;",
                commandType: CommandType.Text));

            return result.FirstOrDefault();
        }

        public async Task SetLastUpdateDate(DateTime dateUpdated)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@dateCalculated", dateUpdated, DbType.Date);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [employer_financial].[EnglishFractionCalculationDate] (DateCalculated) VALUES (@dateCalculated);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetCurrentFractionForSchemes(long accountId, IEnumerable<string> employerReferences)
        {
            var currentFractions = new List<DasEnglishFraction>();
            await WithConnection(async c =>
            {
                foreach (var employerReference in employerReferences)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@accountId", accountId, DbType.Int64);
                    parameters.Add("@empRef", employerReference, DbType.String);

                    var currentFraction = await c.QueryAsync<DasEnglishFraction>(
                        sql: "[employer_financial].[GetCurrentFractionForScheme]",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);

                    currentFractions.Add(currentFraction.FirstOrDefault());
                }
                return 0;
            });

            return currentFractions;
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", employerReference, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "SELECT * FROM [employer_financial].[EnglishFraction] WHERE EmpRef = @empRef ORDER BY DateCalculated desc;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result;
        }

        public async Task CreateEmployerFraction(DasEnglishFraction fractions, string employerReference)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpRef", employerReference, DbType.String);
                parameters.Add("@Amount", fractions.Amount, DbType.Decimal);
                parameters.Add("@dateCalculated", fractions.DateCalculated, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [employer_financial].[EnglishFraction] (EmpRef, DateCalculated, Amount) VALUES (@empRef, @dateCalculated, @amount);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}

