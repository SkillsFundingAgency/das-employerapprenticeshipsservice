using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper
{
    class DbDirectRepository : BaseRepository
    {
        public DbDirectRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            // Just call base
        }

        public Task<EmployerAccountOutput> GetAccountDetailsAsync(string accountName)
        {
            const string sql = "SELECT Id, HashedId, PublicHashedId FROM [employer_account].Account WHERE Name = @accountName";
            return WithConnection(conn => conn.QueryFirstOrDefaultAsync<EmployerAccountOutput>(sql, new {accountName}));
        }
    }
}