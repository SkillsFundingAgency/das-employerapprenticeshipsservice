using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Jobs.Data;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;
using IMembershipRepository = SFA.DAS.EmployerAccounts.Jobs.Data.IMembershipRepository;

namespace SFA.DAS.EmployerAccounts.Jobs.UnitTests.StartupJobs
{
    [TestFixture]
    [Parallelizable]
    public class SeedAccountUsersJobTests : FluentTest<SeedAccountUsersJobTestsFixture>
    {
        [Test]
        public Task Run_WhenRunningAfterJobHasSuccessfullyCompleted_ThenShouldImmediatelyReturnAndDoNoUpdatesOrQueries()
        {
            return TestAsync(f => f.SetJobAsAlreadyRun(),
                f => f.Run(), f => f.VerifyUserQueryNotRun().VerifyMarkAsPopulatedNotRun());
        }

        [Test]
        public Task Run_WhenRunningJobFirstTime_ThenShouldQueryForUsers()
        {
            return TestAsync(f => f.Run(), f => f.VerifyUserQueryWasRun());
        }

        [Test]
        public Task Run_WhenRunningJobFirstTime_ThenShouldMarkJobAsPopulated()
        {
            return TestAsync(f => f.Run(), f => f.VerifyMarkAsPopulatedWasRun());
        }

        [Test]
        public Task Run_WhenRunningJobFirstTimeAndWeFindTwoNewUsers_ThenShouldAddEachUser()
        {
            return TestAsync(f => f.CreateTwoNewUsers(), f => f.Run(), 
                f => f.VerifyAddDocumentWasRun(2).VerifyUserWasMappedCorrectly(f.NewUser1).VerifyUserWasMappedCorrectly(f.NewUser2));
        }

        [Test]
        public Task Run_WhenRunningJobFirstTimeAndWeFindTwoUsersButFirstIsAlreadyInReadStore_ThenShouldAddSecondUserOnly()
        {
            return TestAsync(f => f.CreateTwoNewUsers().AddFirstUserToReadStore(), f => f.Run(),
                f => f.VerifyAddDocumentWasRun(1).VerifyUserWasMappedCorrectly(f.NewUser2));
        }
    }

    public class SeedAccountUsersJobTestsFixture
    {
        internal Mock<IAccountUsersRepository> AccountUsersRepository { get; set; }
        internal Mock<IMembershipRepository> MembershipRepository { get; set; }
        internal Mock<IJobHistoryRepository> JobHistoryRepository { get; set; }
        public Mock<ILogger> Logger { get; set; }

        public ICollection<MembershipUser> Users = new List<MembershipUser>();
        internal ICollection<AccountUser> ReadStoreUsers = new List<AccountUser>();

        public MembershipUser NewUser1 = new MembershipUser { AccountId = 1100, Role = 1, UserId = 1111, UserRef = Guid.NewGuid() };
        public MembershipUser NewUser2 = new MembershipUser { AccountId = 2100, Role = 2, UserId = 2222, UserRef = Guid.NewGuid() };
        internal SeedAccountUsersJob SeedAccountUsersJob { get; set; }

        private readonly string _jobName = typeof(SeedAccountUsersJob).Name;

        public SeedAccountUsersJobTestsFixture()
        {
            AccountUsersRepository = new Mock<IAccountUsersRepository>();
            AccountUsersRepository.SetupInMemoryCollection(ReadStoreUsers);

            MembershipRepository = new Mock<IMembershipRepository>();
            MembershipRepository.Setup(x => x.GetAllAccountUsers()).ReturnsAsync(Users);

            JobHistoryRepository = new Mock<IJobHistoryRepository>();

            Logger = new Mock<ILogger>();

            SeedAccountUsersJob =
                new SeedAccountUsersJob(AccountUsersRepository.Object, MembershipRepository.Object, 
                    JobHistoryRepository.Object, Logger.Object);
        }

        public Task Run()
        {
            return SeedAccountUsersJob.Run();
        }

        public SeedAccountUsersJobTestsFixture SetJobAsAlreadyRun()
        {
            JobHistoryRepository.Setup(x => x.HasJobRun(_jobName)).ReturnsAsync(true);

            return this;
        }

        public SeedAccountUsersJobTestsFixture CreateTwoNewUsers()
        {
            Users.Add(NewUser1);
            Users.Add(NewUser2);

            return this;
        }

        public SeedAccountUsersJobTestsFixture AddFirstUserToReadStore()
        {
            ReadStoreUsers.Add(ObjectActivator.CreateInstance<AccountUser>()
                .Set(x=>x.UserRef, NewUser1.UserRef)
                .Set(x=>x.AccountId, NewUser1.AccountId));

            return this;
        }

        public SeedAccountUsersJobTestsFixture VerifyUserQueryNotRun()
        {
            MembershipRepository.Verify(x => x.GetAllAccountUsers(), Times.Never);

            return this;
        }

        public SeedAccountUsersJobTestsFixture VerifyAddDocumentWasRun(short times)
        {
            AccountUsersRepository.Verify(x => x.Add(It.IsAny<AccountUser>(), null, CancellationToken.None), Times.Exactly(times));

            return this;
        }

        public SeedAccountUsersJobTestsFixture VerifyUserWasMappedCorrectly(MembershipUser user)
        {
            AccountUsersRepository.Verify(x => x.Add(It.Is<AccountUser>(p => p.AccountId == user.AccountId &&
                                                                             p.UserRef == user.UserRef &&
                                                                             p.Role.Value == (UserRole) user.Role), 
                null, 
                CancellationToken.None), Times.Once);

            return this;
        }


        public SeedAccountUsersJobTestsFixture VerifyUserQueryWasRun()
        {
            MembershipRepository.Verify(x => x.GetAllAccountUsers(), Times.Once);

            return this;
        }

        public SeedAccountUsersJobTestsFixture VerifyMarkAsPopulatedNotRun()
        {
            JobHistoryRepository.Verify(x => x.MarkJobAsRan(_jobName), Times.Never);

            return this;
        }
        public SeedAccountUsersJobTestsFixture VerifyMarkAsPopulatedWasRun()
        {
            JobHistoryRepository.Verify(x => x.MarkJobAsRan(_jobName), Times.Once);

            return this;
        }
    }
}