using System;
using SFA.DAS.CosmosDb;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using IMembershipRepository = SFA.DAS.EmployerAccounts.Jobs.Data.IMembershipRepository;

namespace SFA.DAS.EmployerAccounts.Jobs.StartupJobs
{
    public class SeedAccountUsersJob
    {
        private readonly IAccountUsersRepository _accountUsersRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly Lazy<JobDbContext> _jobDb;
        private readonly ILogger _logger;
        private readonly string _jobName;

        public SeedAccountUsersJob(IAccountUsersRepository accountUsersRepository, IMembershipRepository membershipRepository, 
            Lazy<JobDbContext> jobDb, ILogger logger)
        {
            _accountUsersRepository = accountUsersRepository;
            _membershipRepository = membershipRepository;
            _jobDb = jobDb;
            _logger = logger;
            _jobName = typeof(SeedAccountUsersJob).Name;
        }

        [NoAutomaticTrigger]
        public async Task Run()
        {
            _logger.LogInformation($"Job '{_jobName}' started");

            if (await _jobDb.Value.Jobs.AnyAsync(j=>j.Name == _jobName))
            {
                _logger.LogInformation($"Job '{_jobName}' has already been run");
                return;
            }

            var users = await _membershipRepository.GetAllAccountUsers();

            _logger.LogInformation("Migrating users into the read store"); 

            var populateMessageId = Guid.NewGuid().ToString();

            foreach (var user in users)
            {
                var accountUserExists = await CosmosDb.QueryableExtensions.AnyAsync(_accountUsersRepository.CreateQuery(), x => x.AccountId == user.AccountId && x.UserRef == user.UserRef);

                if (!accountUserExists)
                {
                        var document  = new AccountUser(user.UserRef, user.AccountId, (UserRole)user.Role, DateTime.Now, populateMessageId);
                    await _accountUsersRepository.Add(document, null, CancellationToken.None);
                }
            }

            _logger.LogInformation("Finished migrating users into the read store");

            _jobDb.Value.Jobs.Add(new Job(_jobName, DateTime.UtcNow));

            await _jobDb.Value.SaveChangesAsync();
        }
    }
}