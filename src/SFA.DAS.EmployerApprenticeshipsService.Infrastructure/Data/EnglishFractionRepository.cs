using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EnglishFractionRepository : BaseRepository, IEnglishFractionRepository
    {
        public EnglishFractionRepository(LevyDeclarationProviderConfiguration configuration)
            : base(configuration)
        {
        }
        
        public async Task<DasEnglishFraction> GetLatest(string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "SELECT Id, DateCalculated, Amount FROM [levy].[EnglishFraction] WHERE empRef = @EmpRef ORDER BY DateCalculated DESC;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.FirstOrDefault();
        }

        public Task Save(DasEnglishFraction fraction)
        {
            throw new System.NotImplementedException();
        }

        public async Task<DasEnglishFraction> GetEmployerFraction(DateTime dateCalculated, string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@dateCalculated", dateCalculated, DbType.DateTime);
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "SELECT * FROM [levy].[EnglishFraction] WHERE EmpRef = @empRef AND DateCalculated = @dateCalculated;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task CreateEmployerFraction(DasEnglishFraction fractions, string empRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpRef", empRef, DbType.String);
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

