using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

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
    }
}