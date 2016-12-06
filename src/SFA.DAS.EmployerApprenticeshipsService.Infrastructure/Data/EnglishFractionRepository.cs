using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EnglishFractionRepository : BaseRepository, IEnglishFractionRepository
    {
        public EnglishFractionRepository(LevyDeclarationProviderConfiguration configuration)
            : base(configuration)
        {
        }
        
        public async Task<DateTime> GetLastUpdateDate()
        {
            var result = await WithConnection(async c => await c.QueryAsync<DateTime>(
                sql: "SELECT Top(1) DateCalculated FROM [levy].[EnglishFractionCalculationDate] ORDER BY DateCalculated DESC;",
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
                    sql: "INSERT INTO [levy].[EnglishFractionCalculationDate] (DateCalculated) VALUES (@dateCalculated);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<DasEnglishFraction> GetEmployerFraction(DateTime dateCalculated, string employerReference)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@dateCalculated", dateCalculated, DbType.DateTime);
                parameters.Add("@empRef", employerReference, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "SELECT * FROM [levy].[EnglishFraction] WHERE EmpRef = @empRef AND DateCalculated = @dateCalculated;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", employerReference, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "SELECT * FROM [levy].[EnglishFraction] WHERE EmpRef = @empRef ORDER BY DateCalculated desc;",
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
                    sql: "INSERT INTO [levy].[EnglishFraction] (EmpRef, DateCalculated, Amount) VALUES (@empRef, @dateCalculated, @amount);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}

