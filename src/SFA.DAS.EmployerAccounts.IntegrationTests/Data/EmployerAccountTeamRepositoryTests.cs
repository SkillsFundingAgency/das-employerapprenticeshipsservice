using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.IntegrationTests.Data
{
    [TestFixture]
    public class EmployerAccountTeamRepositoryTests
    {
        [Test]
        public async Task Constructor_Valid_ShouldNotThrowException()
        {
            var fixtures = new EmployerAccountTeamRepositoryTestFixtures();

            await fixtures.CheckEmployerAccountTeamRepository(repo => Task.CompletedTask);

            Assert.Pass("Did not get an exception");
        }

        [Test]
        public Task GetMember_ForActiveUser_ShouldExecuteQueryWithoutExecption()
        {
            // Arrange
            string email = $"{Guid.NewGuid().ToString()}@foo.com";

            var fixtures = new EmployerAccountTeamRepositoryTestFixtures()
                                .WithNewAccountAndActiveUser(email, Role.Owner, out var hashedId);

            return fixtures.CheckEmployerAccountTeamRepository(async repo =>
            {
                var member = await repo.GetMember(hashedId, email, true);
                Assert.IsNotNull(member);
                Assert.AreEqual(email, member.Email);
            });
        }

        [TestCase(InvitationStatus.Pending, 5)]
        [TestCase(InvitationStatus.Deleted, 5)]
        [TestCase(InvitationStatus.Expired, -1)]
        public Task GetMember_ForNonActiveUser_ShouldExecuteQueryWithoutExecption(InvitationStatus status, int daysUntilExpiry)
        {
            // Arrange
            string email = $"{Guid.NewGuid().ToString()}@foo.com";

            var fixtures = new EmployerAccountTeamRepositoryTestFixtures()
                .WithInvitation(status, DateTime.Now.AddDays(daysUntilExpiry), email, out var hashedId);

            return fixtures.CheckEmployerAccountTeamRepository(async repo =>
            {
                var member = await repo.GetMember(hashedId, email, true);
                Assert.IsNull(member);
            });
        }
    }

    class EmployerAccountTeamRepositoryTestFixtures
    {
        public EmployerAccountTeamRepositoryTestFixtures()
        {
            EmployerAccountsConfiguration =
                ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>(Constants.ServiceName);

            LoggerMock = new Mock<ILog>();
        }

        public EmployerAccountsConfiguration EmployerAccountsConfiguration { get; }

        public Mock<ILog> LoggerMock { get; private set; }
        public ILog Logger => LoggerMock.Object;

        public EmployerAccountTeamRepositoryTestFixtures WithNewAccountAndActiveUser(string email, Role role, out string hashedId)
        {
            using (var db = CreateDbContext())
            {
                var account = CreateAccount(db);
                hashedId = account.HashedId;

                var user = db.Users.Create();
                user.FirstName = email;
                user.LastName = "User created for unit test";
                user.Email = email;
                user.Ref = Guid.NewGuid();

                var membership = db.Memberships.Create();
                membership.Account = account;
                membership.User = user;
                membership.Role = role;

                db.Users.Add(user);
                db.Memberships.Add(membership);
                db.SaveChanges();
            }

            return this;
        }

        public EmployerAccountTeamRepositoryTestFixtures WithInvitation(
            InvitationStatus status, 
            DateTime expiryDate,
            string email,
            out string hashedId)
        {
            using (var db = CreateDbContext())
            {
                var account = CreateAccount(db);
                hashedId = account.HashedId;

                db.SaveChanges();

                var invitation = new Invitation
                {
                    AccountId = account.Id,
                    Status = status,
                    ExpiryDate = expiryDate,
                    Email = email
                };

                CheckInvitationRepository(repo => repo.Create(invitation));
            }

            return this;
        }

        public Task CheckEmployerAccountTeamRepository(Func<EmployerAccountTeamRepository, Task> action)
        {
            return RunWithTransaction(
                repositoryCreator: db => new EmployerAccountTeamRepository(EmployerAccountsConfiguration, Logger,
                    new Lazy<EmployerAccountsDbContext>(() => db)),
                action: action);
        }

        public Task CheckInvitationRepository(Func<InvitationRepository, Task> action)
        {
            return RunWithTransaction(
                repositoryCreator: db => new InvitationRepository(EmployerAccountsConfiguration, Logger,
                    new Lazy<EmployerAccountsDbContext>(() => db)),
                action: action);
        }

        private async Task RunWithTransaction<TRepository>(
            Func<EmployerAccountsDbContext, TRepository> repositoryCreator,
            Func<TRepository, Task> action) where TRepository : BaseRepository
        {
            using (var db = CreateDbContext())
            {
                db.Database.BeginTransaction();

                try
                {
                    var repo = repositoryCreator(db);
                    await action(repo);

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw;
                }
            }
        }

        private Account CreateAccount(EmployerAccountsDbContext db)
        {
            var hashedId = GetNextHashedIdForTests(db);
            var account = db.Accounts.Create();
            account.CreatedDate = DateTime.Now;
            account.HashedId = hashedId;
            account.Name = $"Account created for unit test {hashedId}";
            db.Accounts.Add(account);

            return account;
        }

        private string GetNextHashedIdForTests(EmployerAccountsDbContext dbContext)
        {
            var regex = new Regex("ZZ[0-9]{4}");

            var maxHashedId = dbContext.Accounts
                .Where(ac => ac.HashedId.StartsWith("ZZ"))
                .AsEnumerable()
                .Where(ac => regex.IsMatch(ac.HashedId))
                .Max(ac => ac.HashedId);

            if (string.IsNullOrWhiteSpace(maxHashedId))
            {
                return "ZZ0001";
            }
            else
            {
                var intPart = int.Parse(maxHashedId.Substring(2, 4)) + 1;
                return $"ZZ{intPart:D4}";
            }
        }

        private EmployerAccountsDbContext CreateDbContext()
        {
            return new EmployerAccountsDbContext(EmployerAccountsConfiguration.DatabaseConnectionString);
        }
    }
}
