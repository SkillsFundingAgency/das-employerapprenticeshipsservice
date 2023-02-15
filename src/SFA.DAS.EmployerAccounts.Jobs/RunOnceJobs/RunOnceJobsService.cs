using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;

public class RunOnceJobsService : IRunOnceJobsService
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly ILogger<RunOnceJobsService> _logger;

    public RunOnceJobsService(Lazy<EmployerAccountsDbContext> db, ILogger<RunOnceJobsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task RunOnce(string jobName, Func<Task> function)
    {
        _logger.LogInformation($"RunOnceJob '{jobName}' started");

        if ((await _db.Value.RunOnceJobs.SingleOrDefaultAsync(j=>j.Name == jobName) != null))
        {
            _logger.LogInformation($"Job '{jobName}' has already been run");
            return;
        }

        await function();

        _logger.LogInformation($"RunOnceJob '{jobName}' finished");
        _db.Value.RunOnceJobs.Add(new RunOnceJob(jobName, DateTime.UtcNow));
        await _db.Value.SaveChangesAsync();
    }
}