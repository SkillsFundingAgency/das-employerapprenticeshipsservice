using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Portal.Jobs.StartupJobs
{
    // there's a separate task/pr for this, but we need a 'hello world' job to test the host
    public class CreateReadStoreDatabaseJob
    {
        // singleton attribute requires a real storage account, so webjobs has access to blobs (so emulator doesn't work)
        // set env variables AzureWebJobsDashboard & AzureWebJobsStorage (for now) to a real storage account
        // add to readme.md?
        //todo: use secret manager, rather than env variables (easier to have different settings for different projects)
        // ^^ see https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
        [NoAutomaticTrigger]
        [Singleton]
        public Task CreateReadStoreDatabase(ExecutionContext executionContext, ILogger logger)
        {
            logger.LogInformation($"{executionContext.InvocationId}: Helo Byd");
            return Task.CompletedTask;
        }
    }
}
