using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EmployerSchemesRepository : BaseRepository, IEmployerSchemesRepository
    {
        public EmployerSchemesRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
        }

        public async Task<Schemes> GetSchemesByEmployerId(long employerId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", employerId, DbType.Int64);

                return await c.QueryAsync<Scheme>(
                    sql: "SELECT * FROM [account].[Paye] WHERE AccountId = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
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
                parameters.Add("@ref", empref, DbType.String);

                return await c.QueryAsync<Scheme>(
                    sql: "SELECT * FROM [account].[Paye] WHERE Ref = @ref;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();

        }
    }
}
