using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs
{
    public class SeedAccountSignedAgreementsJob
    {
        private readonly IRunOnceJobsService _runOnceJobsService;
        private readonly IAccountSignedAgreementsRepository _accountSignedAgreementsRepository;
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly ILogger _logger;
        private readonly string _jobName;

        public SeedAccountSignedAgreementsJob(IRunOnceJobsService runOnceJobsService, IAccountSignedAgreementsRepository accountSignedAgreementsRepository, Lazy<EmployerAccountsDbContext> db, ILogger logger)
        {
            _runOnceJobsService = runOnceJobsService;
            _accountSignedAgreementsRepository = accountSignedAgreementsRepository;
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
            var agreements = _db.Value.Agreements
                .Include(x => x.AccountLegalEntity)
                .Include(x => x.Template)
                .Where(x => x.SignedDate != null)
                .ToList();

            _logger.LogInformation("Migrating signed agreements into the read store");

            foreach (var agreement in agreements)
            {
                var agreementExists = await CosmosDb.QueryableExtensions.AnyAsync(
                    _accountSignedAgreementsRepository.CreateQuery(),
                    x => x.AccountId == agreement.AccountLegalEntity.AccountId && x.AgreementType == agreement.Template.AgreementType.ToString() && x.AgreementVersion == agreement.Template.VersionNumber);

                if (!agreementExists)
                {
                    var document = new AccountSignedAgreement(agreement.AccountLegalEntity.AccountId, agreement.Template.VersionNumber, agreement.Template.AgreementType.ToString(), Guid.NewGuid());
                    await _accountSignedAgreementsRepository.Add(document, null, CancellationToken.None);
                }
            }

            _logger.LogInformation("Finished migrating signed agreements into the read store");
        }
    }
}