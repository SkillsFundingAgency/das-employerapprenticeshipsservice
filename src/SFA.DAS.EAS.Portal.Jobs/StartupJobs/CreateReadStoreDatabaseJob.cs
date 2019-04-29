using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Portal.Jobs.StartupJobs
{
    // there's a separate task/pr for this, but we need a 'hello world' job to test the host
    public class CreateReadStoreDatabaseJob
    {
        // singleton attribute requires a real storage account, so webjobs has access to blobs (so emulator doesn't work)
        [NoAutomaticTrigger]
        [Singleton]
        public Task CreateReadStoreDatabase(ILogger logger)
        {
            logger.LogInformation("Helo Byd");
            return Task.CompletedTask;
        }
    }
}
