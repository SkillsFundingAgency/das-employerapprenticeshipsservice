using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.Jobs.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.StartupJobs
{
    internal class PopulateAccountUsersInCollection
    {
        private readonly IAccountUsersRepository _accountUsersRepository;
        private readonly IPopulateRepository _populateRepository;

        public PopulateAccountUsersInCollection(IAccountUsersRepository accountUsersRepository, IPopulateRepository populateRepository)
        {
            _accountUsersRepository = accountUsersRepository;
            _populateRepository = populateRepository;
        }

        [NoAutomaticTrigger]
        public async Task Run()
        {
            if (await _populateRepository.AlreadyPopulated())
            {
                return;
            }

            var populateMessageId = Guid.NewGuid().ToString();

            // Read In Users
            var users = await _populateRepository.GetAllAccountUsers();

            // For each user add record to collection if not exists
            foreach (var user in users)
            {
                if (await _accountUsersRepository.CreateQuery().AnyAsync(x => x.AccountId == user.AccountId && x.UserRef == user.UserRef) == false)
                {
                    var document  = new AccountUser(user.UserRef, user.AccountId, new HashSet<UserRole> { (UserRole)user.Role }, DateTime.Now, populateMessageId);
                    await _accountUsersRepository.Add(document, null, CancellationToken.None);
                }
            }

            await _populateRepository.MarkAsPopulated();
        }
    }
}