using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.Jobs.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.StartupJobs
{
    public class PopulateAccountUsersInCollectionJob
    {
        private readonly IAccountUsersRepository _accountUsersRepository;
        private readonly IPopulateRepository _populateRepository;
        private readonly ILogger _logger;
        private readonly string _jobName;


        public PopulateAccountUsersInCollectionJob(IAccountUsersRepository accountUsersRepository, IPopulateRepository populateRepository, ILogger logger)
        {
            _accountUsersRepository = accountUsersRepository;
            _populateRepository = populateRepository;
            _logger = logger;
            _jobName = typeof(PopulateAccountUsersInCollectionJob).Name;
        }

        [NoAutomaticTrigger]
        public async Task Run()
        {
            if (await _populateRepository.HasJobRun(_jobName))
            {
                _logger.LogInformation($"Job '{_jobName}' has already been run");
                return;
            }

            var users = await _populateRepository.GetAllAccountUsers();

            _logger.LogInformation("Migrating users into the read store"); 

            var populateMessageId = Guid.NewGuid().ToString();

            foreach (var user in users)
            {
                if (await _accountUsersRepository.CreateQuery().AnyAsync(x => x.AccountId == user.AccountId && x.UserRef == user.UserRef) == false)
                {
                    var document  = new AccountUser(user.UserRef, user.AccountId, new HashSet<UserRole> { (UserRole)user.Role }, DateTime.Now, populateMessageId);
                    await _accountUsersRepository.Add(document, null, CancellationToken.None);
                }
            }

            _logger.LogInformation("Finished Migrating users into the read store");

            await _populateRepository.MarkJobAsRan(_jobName);
        }
    }
}