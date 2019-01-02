using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs
{
    public class RunOnceService : IRunOnceService
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly ILogger _logger;

        public RunOnceService(Lazy<EmployerAccountsDbContext> db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task RunOnce(string jobName, Func<Task> function)
        {
            _logger.LogInformation($"RunOnceJob '{jobName}' started");

            if (await _db.Value.RunOnceJobs.AnyAsync(j=>j.Name == jobName))
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
}