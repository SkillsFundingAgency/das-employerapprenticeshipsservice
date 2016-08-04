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
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        public DasLevyRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
        }

        public async Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.String);
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "SELECT Amount, SubmissionId AS Id, SubmissionType, SubmissionDate AS [Date] FROM [dbo].[LevyDeclaration] WHERE empRef = @EmpRef and SubmissionId = @Id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Amount", dasDeclaration.Amount, DbType.Decimal);
                parameters.Add("@EmpRef", empRef, DbType.String);
                parameters.Add("@SubmissionDate", dasDeclaration.Date, DbType.DateTime);
                parameters.Add("@SubmissionId", dasDeclaration.Id, DbType.String);
                parameters.Add("@SubmissionType", dasDeclaration.SubmissionType, DbType.String);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [dbo].[LevyDeclaration] (Amount, empRef, SubmissionDate, SubmissionId, SubmissionType) VALUES (@Amount, @EmpRef, @SubmissionDate, @SubmissionId, @SubmissionType);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<DasEnglishFractions> GetEmployerFraction(DateTime dateCalculated, string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@dateCalculated", dateCalculated, DbType.DateTime);
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasEnglishFractions>(
                    sql: "SELECT * FROM [dbo].[EnglishFraction] WHERE EmpRef = @empRef AND DateCalculated = @dateCalculated;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task CreateEmployerFraction(DasEnglishFractions fractions, string empRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpRef", empRef, DbType.String);
                parameters.Add("@Amount", fractions.Amount, DbType.Decimal);
                parameters.Add("@dateCalculated", fractions.DateCalculated, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [dbo].[EnglishFraction] (EmpRef, DateCalculated, Amount) VALUES (@empRef, @dateCalculated, @amount);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(int accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int32);

                return await c.QueryAsync<LevyDeclarationView>(
                    sql: "SELECT * from [dbo].[GetLevyDeclarations] WHERE [AccountId] = @accountId ORDER BY [SubmissionDate] ASC;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }
    }
}

