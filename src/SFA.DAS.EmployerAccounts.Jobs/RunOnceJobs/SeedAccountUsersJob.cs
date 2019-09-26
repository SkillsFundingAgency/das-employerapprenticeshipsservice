﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs
{
    public class SeedAccountUsersJob
    {
        private readonly IRunOnceJobsService _runOnceJobsService;
        private readonly IAccountUsersRepository _accountUsersRepository;
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly ILogger _logger;
        private readonly string _jobName;

        public SeedAccountUsersJob(IRunOnceJobsService runOnceJobsService, IAccountUsersRepository accountUsersRepository, Lazy<EmployerAccountsDbContext> db, ILogger logger)
        {
            _runOnceJobsService = runOnceJobsService;
            _accountUsersRepository = accountUsersRepository;
            _db = db;
            _logger = logger;
            _jobName = typeof(SeedAccountUsersJob).Name;
        }

        [NoAutomaticTrigger]
        public Task Run()
        {
            return _runOnceJobsService.RunOnce(_jobName, MigrateUsers);
        }

        public async Task MigrateUsers()
        {
            var users = _db.Value.Memberships.Include(m => m.User).Where(x => x.Role != Role.None).ToList();

            _logger.LogInformation("Migrating users into the read store");

            var populateMessageId = Guid.NewGuid().ToString();

            foreach (var user in users)
            {
                var accountUserExists = await CosmosDb.QueryableExtensions.AnyAsync(
                    _accountUsersRepository.CreateQuery(),
                    x => x.AccountId == user.AccountId && x.UserRef == user.User.Ref);

                if (!accountUserExists)
                {
                    var document = new AccountUser(user.User.Ref, user.AccountId, (UserRole) user.Role,
                        DateTime.UtcNow, populateMessageId);
                    await _accountUsersRepository.Add(document, null, CancellationToken.None);
                }
            }

            _logger.LogInformation("Finished migrating users into the read store");
        }
    }
}