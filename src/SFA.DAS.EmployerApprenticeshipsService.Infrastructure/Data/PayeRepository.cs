using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class PayeRepository : BaseRepository, IPayeRepository
    {
        public PayeRepository(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        public async Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string reference)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);
                parameters.Add("@Ref", reference, DbType.String);

                return await c.QueryAsync<PayeSchemeView>(
                    sql: "[employer_account].[GetPayeForAccount_ByRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
            return result.SingleOrDefault();
        }

        public async Task<Paye> GetPayeSchemeByRef(string payeRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Ref", payeRef, DbType.String);

                return await c.QueryAsync<Paye>(
                    sql: "[employer_account].[GetPaye_ByRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
            return result.SingleOrDefault();
        }

        public async Task UpdatePayeSchemeName(string payeRef, string refName)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Ref", payeRef, DbType.String);
                parameters.Add("@RefName", refName, DbType.String);

                return await c.ExecuteAsync(
                    sql: "[employer_account].[UpdatePayeName_ByRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }
    }
}