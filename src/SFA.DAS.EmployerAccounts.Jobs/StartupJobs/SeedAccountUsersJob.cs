using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.Data;
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
        private readonly IJobHistoryRepository _jobHistoryRepository;
        private readonly ILogger _logger;
        private readonly string _jobName;

        public SeedAccountUsersJob(IAccountUsersRepository accountUsersRepository, IMembershipRepository membershipRepository, 
            IJobHistoryRepository jobHistoryRepository, ILogger logger)
        {
            _accountUsersRepository = accountUsersRepository;
            _membershipRepository = membershipRepository;
            _jobHistoryRepository = jobHistoryRepository;
            _logger = logger;
            _jobName = typeof(SeedAccountUsersJob).Name;
        }

        [NoAutomaticTrigger]
        public async Task Run()
        {
            _logger.LogInformation($"Job '{_jobName}' started");

            if (await _jobHistoryRepository.HasJobRun(_jobName))
            {
                _logger.LogInformation($"Job '{_jobName}' has already been run");
                return;
            }

            var users = await _membershipRepository.GetAllAccountUsers();

            _logger.LogInformation("Migrating users into the read store"); 

            var populateMessageId = Guid.NewGuid().ToString();

            foreach (var user in users)
            {
                var accountUserExists = await _accountUsersRepository.CreateQuery().AnyAsync(x => x.AccountId == user.AccountId && x.UserRef == user.UserRef);

                if (!accountUserExists)
                {
                        var document  = new AccountUser(user.UserRef, user.AccountId, (UserRole)user.Role, DateTime.Now, populateMessageId);
                    await _accountUsersRepository.Add(document, null, CancellationToken.None);
                }
            }

            _logger.LogInformation("Finished migrating users into the read store");

            await _jobHistoryRepository.MarkJobAsRan(_jobName);
        }
    }
}