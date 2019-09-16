using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.Jobs.UnitTests.OneOffJobs
{
    [TestFixture]
    [Parallelizable]
    public class SeedAccountSignedAgreementsJobTests : FluentTest<SeedAccountSignedAgreementsJobTestsFixture>
    {
        [Test]
        public Task Run_WhenRunningJob_ThenShouldOnlyAddAgreementsWhichHaveBeenSigned()
        {
            return TestAsync(f => f.Run(),
                f => f.VerifyAddDocumentWasRun(1).VerifyUserWasMappedCorrectly(f.SignedAgreement));
        }

        [Test]
        public Task Run_WhenRunningJob_ThenShouldNotAddAgreementsWhichHaveBeenSignedAndAlreadyAreInTheReadstore()
        {
            return TestAsync(f => f.AddExistingSignedAgreementToReadstore(),f => f.Run(),
                f => f.VerifyAddDocumentWasRun(0));
        }

        [Test]
        public Task Run_WhenRunningJob_ThenShouldPassInTheCorrectJobName()
        {
            return TestAsync(f => f.Run(),
                f => f.RunOnceService.Verify(x => x.RunOnce("SeedAccountUsersJob", It.IsAny<Func<Task>>())));
        }
    }

    public class SeedAccountSignedAgreementsJobTestsFixture
    {
        internal Mock<IRunOnceJobsService> RunOnceService { get; set; }
        internal Mock<IAccountSignedAgreementsRepository> AccountSignedAgreementsRepository { get; set; }
        internal Mock<EmployerAccountsDbContext> EmployerAccountsDbContext { get; set; }
        public Mock<ILogger> Logger { get; set; }

        public ICollection<EmployerAgreement> Agreements = new List<EmployerAgreement>();

        public ICollection<AccountSignedAgreement> ReadStoreAgreements = new List<AccountSignedAgreement>();

        public EmployerAgreement SignedAgreement = new EmployerAgreement{ AccountLegalEntity = new AccountLegalEntity{ AccountId = 112 }, Template = new AgreementTemplate{ AgreementType = AgreementType.Levy, VersionNumber = 3 }, SignedDate = DateTime.Today.AddDays(-1) };
        public EmployerAgreement UnsignedAgreement = new EmployerAgreement { AccountLegalEntity = new AccountLegalEntity { AccountId = 114 }, Template = new AgreementTemplate { AgreementType = AgreementType.Levy, VersionNumber = 3 } };

        internal SeedAccountSignedAgreementsJob SeedAccountSignedAgreementsJob { get; set; }

        public List<EmployerAgreement> AgreementsToMigrate;

        private readonly string _jobName = typeof(SeedAccountSignedAgreementsJob).Name;

        private readonly DbSetStub<EmployerAgreement> _agreementsDbSet;

        public SeedAccountSignedAgreementsJobTestsFixture()
        {
            RunOnceService = new Mock<IRunOnceJobsService>();
            RunOnceService.Setup(x => x.RunOnce(It.IsAny<string>(), It.IsAny<Func<Task>>()))
                .Returns((string jobName, Func<Task> function) => function());

            AccountSignedAgreementsRepository = new Mock<IAccountSignedAgreementsRepository>();
            AccountSignedAgreementsRepository.SetupInMemoryCollection(ReadStoreAgreements);

            AgreementsToMigrate = new List<EmployerAgreement> { SignedAgreement, UnsignedAgreement };
            _agreementsDbSet = new DbSetStub<EmployerAgreement>(AgreementsToMigrate);

            EmployerAccountsDbContext = new Mock<EmployerAccountsDbContext>();
            EmployerAccountsDbContext.Setup(x => x.Agreements).Returns(_agreementsDbSet);

            Logger = new Mock<ILogger>();

            SeedAccountSignedAgreementsJob = new SeedAccountSignedAgreementsJob(RunOnceService.Object, AccountSignedAgreementsRepository.Object, new Lazy<EmployerAccountsDbContext>(() => EmployerAccountsDbContext.Object), Logger.Object);
        }

        public Task Run()
        {
            return SeedAccountSignedAgreementsJob.Run();
        }

        public SeedAccountSignedAgreementsJobTestsFixture AddExistingSignedAgreementToReadstore()
        {

            ReadStoreAgreements.Add(ObjectActivator.CreateInstance<AccountSignedAgreement>()
                .Set(x => x.AccountId, SignedAgreement.AccountLegalEntity.AccountId)
                .Set(x => x.AgreementVersion, SignedAgreement.Template.VersionNumber)
                .Set(x => x.AgreementType, SignedAgreement.Template.AgreementType.ToString()));

            return this;
        }

        public SeedAccountSignedAgreementsJobTestsFixture VerifyAddDocumentWasRun(short times)
        {
            AccountSignedAgreementsRepository.Verify(x => x.Add(It.IsAny<AccountSignedAgreement>(), null, CancellationToken.None), Times.Exactly(times));

            return this;
        }

        public SeedAccountSignedAgreementsJobTestsFixture VerifyUserWasMappedCorrectly(EmployerAgreement agreement)
        {
            AccountSignedAgreementsRepository.Verify(x => x.Add(It.Is<AccountSignedAgreement>(p => p.AccountId == agreement.AccountLegalEntity.AccountId &&
                                                                             p.AgreementType == agreement.Template.AgreementType.ToString() &&
                                                                             p.AgreementVersion == agreement.Template.VersionNumber),
                null,
                CancellationToken.None), Times.Once);

            return this;
        }
    }
}