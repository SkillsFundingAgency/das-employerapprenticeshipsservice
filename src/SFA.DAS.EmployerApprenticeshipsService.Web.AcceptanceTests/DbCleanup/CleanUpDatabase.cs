using System.Data;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup
{
    public class CleanUpDatabase : BaseRepository, ICleanUpDatabase
    {
        public CleanUpDatabase(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }

        public async Task Execute()
        {

            await WithConnection(async c => await c.ExecuteAsync(
                "[dbo].[Cleardown]",
                null,
                commandType: CommandType.StoredProcedure));

            await WithConnection(async c => await c.ExecuteAsync(
                "[dbo].[SeedDataForRoles]",
                null,
                commandType: CommandType.StoredProcedure));

        }
    }
}
