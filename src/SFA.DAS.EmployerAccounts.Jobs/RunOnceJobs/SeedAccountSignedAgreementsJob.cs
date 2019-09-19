using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;
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
        private readonly IConfigurationProvider _configurationProvider;

        public SeedAccountSignedAgreementsJob(IRunOnceJobsService runOnceJobsService, IAccountSignedAgreementsRepository accountSignedAgreementsRepository, Lazy<EmployerAccountsDbContext> db, ILogger logger, IConfigurationProvider configurationProvider)
        {
            _runOnceJobsService = runOnceJobsService;
            _accountSignedAgreementsRepository = accountSignedAgreementsRepository;
            _db = db;
            _logger = logger;
            _configurationProvider = configurationProvider;
            _jobName = typeof(SeedAccountSignedAgreementsJob).Name;
        }

        [NoAutomaticTrigger]
        public Task Run()
        {
            return _runOnceJobsService.RunOnce(_jobName, MigrateAgreements);
        }

        public async Task MigrateAgreements()
        {
            var agreements = _db.Value.Agreements
                .ProjectTo<AgreementDto>(_configurationProvider)
                .Where(x => x.SignedDate != null);

            _logger.LogInformation("Migrating signed agreements into the read store");

            var readStoreAgreements = await CosmosDb.QueryableExtensions.ToListAsync(
                _accountSignedAgreementsRepository.CreateQuery());

            foreach (var agreement in agreements)
            {
                var agreementExists = readStoreAgreements.Any(x => x.AccountId == agreement.AccountId && x.AgreementType == agreement.Template.AgreementType.ToString() && x.AgreementVersion == agreement.Template.VersionNumber);

                if (!agreementExists)
                {
                    var document = new AccountSignedAgreement(agreement.AccountId, agreement.Template.VersionNumber, agreement.Template.AgreementType.ToString(), Guid.NewGuid());
                    await _accountSignedAgreementsRepository.Add(document, null, CancellationToken.None);
                }
            }

            _logger.LogInformation("Finished migrating signed agreements into the read store");
        }
    }
}