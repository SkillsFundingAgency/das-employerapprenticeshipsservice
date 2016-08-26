using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        public DasLevyRepository(LevyDeclarationProviderConfiguration configuration)
            : base(configuration)
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
                    sql: "SELECT LevyDueYtd, SubmissionId AS Id, SubmissionDate AS [Date] FROM [levy].[LevyDeclaration] WHERE empRef = @EmpRef and SubmissionId = @Id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef, long accountId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@LevyDueYtd", dasDeclaration.LevyDueYtd, DbType.Decimal);
                parameters.Add("@LevyAllowanceForYear", dasDeclaration.LevyAllowanceForFullYear, DbType.Decimal);
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@EmpRef", empRef, DbType.String);
                parameters.Add("@PayrollYear", dasDeclaration.PayrollYear, DbType.String);
                parameters.Add("@PayrollMonth", dasDeclaration.PayrollMonth, DbType.Int16);
                parameters.Add("@SubmissionDate", dasDeclaration.Date, DbType.DateTime);
                parameters.Add("@SubmissionId", dasDeclaration.Id, DbType.String);
                
                return await c.ExecuteAsync(
                    sql: "[levy].[CreateDeclaration]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<LevyDeclarationView>(
                    sql: "SELECT * from [levy].[GetLevyDeclarations] WHERE [AccountId] = @accountId ORDER BY [SubmissionDate] ASC;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }
    }
}

