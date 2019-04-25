using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Jobs.StartupJobs
{
    // there's a separate task/pr for this, but we need a 'hello world' job to test the host
    public class CreateReadStoreDatabaseJob
    {
        // as it creates if not exists, then run on startup
        //[NoAutomaticTrigger]
        [Singleton]
        public Task Run(ILogger logger)
        {
            logger.LogInformation("Helo Byd");
            return Task.CompletedTask;
        }
    }
}
