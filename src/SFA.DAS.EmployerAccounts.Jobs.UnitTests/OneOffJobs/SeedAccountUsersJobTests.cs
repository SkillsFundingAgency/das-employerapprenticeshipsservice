using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.Jobs.UnitTests.OneOffJobs
{
    [TestFixture]
    [Parallelizable]
    public class SeedAccountUsersJobTests : Testing.FluentTest<SeedAccountUsersJobTestsFixture>
    {
        [Test]
        public Task Run_WhenRunningJob_ThenShouldAddUsersWhichHaveRealRolesAndIgnoreThoseSetToNone()
        {
            return TestAsync(f => f.Run(),
                f => f.VerifyAddDocumentWasRun(2).VerifyUserWasMappedCorrectly(f.UserOwnerRole).VerifyUserWasMappedCorrectly(f.UserTranasactorRole));
        }

        [Test]
        public Task Run_WhenRunningJobFirstTimeAndWeFindTheFirstIsAlreadyInReadStore_ThenShouldAddTransactorUserOnly()
        {
            return TestAsync(f => f.AddUserOwnerRoleToReadStore(), f => f.Run(),
                f => f.VerifyAddDocumentWasRun(1).VerifyUserWasMappedCorrectly(f.UserTranasactorRole));
        }

        [Test]
        public Task Run_WhenRunningJob_ThenShouldPassInTheCorrectJobName()
        {
            return TestAsync(f => f.Run(),
                f => f.RunOnceService.Verify(x=>x.RunOnce("SeedAccountUsersJob", It.IsAny<Func<Task>>())));
        }


    }

    public class SeedAccountUsersJobTestsFixture
    {
        internal Mock<IRunOnceJobsService> RunOnceService { get; set; }
        internal Mock<IAccountUsersRepository> AccountUsersRepository { get; set; }
        internal Mock<EmployerAccountsDbContext> EmployerAccountsDbContext { get; set; }
        public Mock<ILogger<SeedAccountUsersJob>> Logger { get; set; }

        public ICollection<Membership> Users = new List<Membership>();

        public ICollection<AccountUser> ReadStoreUsers = new List<AccountUser>();

        public Membership UserOwnerRole = new Membership { AccountId = 1100, Role = Role.Owner, UserId = 1111, User = new User { Ref = Guid.NewGuid() } };
        public Membership UserTranasactorRole = new Membership { AccountId = 2100, Role = Role.Transactor, UserId = 1111, User = new User { Ref = Guid.NewGuid() } };
        public Membership UserNoRole = new Membership { AccountId = 2100, Role = Role.None, UserId = 1111, User = new User { Ref = Guid.NewGuid() } };

        internal SeedAccountUsersJob SeedAccountUsersJob { get; set; }

        public List<Membership> UsersToMigrate;

        private readonly string _jobName = typeof(SeedAccountUsersJob).Name;

        private readonly DbSet<Membership> _usersDbSet;

        public SeedAccountUsersJobTestsFixture()
        {
            RunOnceService = new Mock<IRunOnceJobsService>();
            RunOnceService.Setup(x => x.RunOnce(It.IsAny<string>(), It.IsAny<Func<Task>>()))
                .Returns((string jobName, Func<Task> function) => function());

            AccountUsersRepository = new Mock<IAccountUsersRepository>();
            AccountUsersRepository.SetupInMemoryCollection(ReadStoreUsers);

            UsersToMigrate = new List<Membership> { UserOwnerRole, UserTranasactorRole, UserNoRole };
            _usersDbSet = UsersToMigrate.AsQueryable().BuildMockDbSet().Object; ;
            

            EmployerAccountsDbContext = new Mock<EmployerAccountsDbContext>();
            EmployerAccountsDbContext.Setup(x => x.Memberships).Returns(_usersDbSet);

            Logger = new Mock<ILogger<SeedAccountUsersJob>>();

            SeedAccountUsersJob =
                new SeedAccountUsersJob(RunOnceService.Object, AccountUsersRepository.Object, new Lazy<EmployerAccountsDbContext>(() => EmployerAccountsDbContext.Object), Logger.Object);
        }

        public Task Run()
        {
            return SeedAccountUsersJob.Run();
        }

        public SeedAccountUsersJobTestsFixture AddUserOwnerRoleToReadStore()
        {
            ReadStoreUsers.Add(ObjectActivator.CreateInstance<AccountUser>()
                .Set(x => x.UserRef, UserOwnerRole.User.Ref)
                .Set(x => x.AccountId, UserOwnerRole.AccountId));

            return this;
        }

        public SeedAccountUsersJobTestsFixture VerifyAddDocumentWasRun(short times)
        {
            AccountUsersRepository.Verify(x => x.Add(It.IsAny<AccountUser>(), null, CancellationToken.None), Times.Exactly(times));

            return this;
        }

        public SeedAccountUsersJobTestsFixture VerifyUserWasMappedCorrectly(Membership user)
        {
            AccountUsersRepository.Verify(x => x.Add(It.Is<AccountUser>(p => p.AccountId == user.AccountId &&
                                                                             p.UserRef == user.User.Ref &&
                                                                             p.Role.Value == (UserRole)user.Role),
                null,
                CancellationToken.None), Times.Once);

            return this;
        }
    }
}