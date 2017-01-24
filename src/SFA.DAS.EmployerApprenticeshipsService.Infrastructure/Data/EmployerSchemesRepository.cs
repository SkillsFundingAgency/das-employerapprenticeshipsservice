using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class EmployerSchemesRepository : BaseRepository, IEmployerSchemesRepository
    {
        public EmployerSchemesRepository(EmployerApprenticeshipsServiceConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<Schemes> GetSchemesByEmployerId(long employerId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", employerId, DbType.Int64);

                return await c.QueryAsync<Scheme>(
                    sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return new Schemes
            {
                SchemesList = result.ToList()
            };
        }

        public async Task<Scheme> GetSchemeByRef(string empref)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@payeRef", empref, DbType.String);

                return await c.QueryAsync<Scheme>(
                    sql: "[employer_account].[GetPayeSchemesInUse]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();

        }
    }
}
