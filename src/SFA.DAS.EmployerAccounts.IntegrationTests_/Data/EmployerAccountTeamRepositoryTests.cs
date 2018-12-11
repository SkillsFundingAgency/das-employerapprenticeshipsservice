using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.IntegrationTests.Data
{
    [TestFixture]
    public class EmployerAccountTeamRepositoryTests
    {
        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var fixtures = new EmployerAccountTeamRepositoryTestFixtures();

            fixtures.CreateRepository();

            Assert.Pass("Did not get an exception");
        }

        [Test]
        public async Task GetMember_FotActiveUser_ShouldExecuteQueryWithoutExecption()
        {
            // Arrange
            string email = $"{Guid.NewGuid().ToString()}@foo.com";

            var fixtures = new EmployerAccountTeamRepositoryTestFixtures()
                                .WithNewAccountAndActiveUser(email, Role.Owner, out var hashedId);

            var repo = fixtures.CreateRepository();

            // Act
            var member = await repo.GetMember(hashedId, email, true);

            // Assert
            Assert.IsNotNull(member);
            Assert.AreEqual(email, member.Email);
        }
    }

    class EmployerAccountTeamRepositoryTestFixtures
    {
        public EmployerAccountTeamRepositoryTestFixtures()
        {
            EmployerAccountsConfiguration =
                ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>(Constants.ServiceName, "1.0");

            LoggerMock = new Mock<ILog>();

            LazyDbContext = new Lazy<EmployerAccountsDbContext>(CreateDbContext);
        }

        public EmployerAccountsConfiguration EmployerAccountsConfiguration { get; }

        public Mock<ILog> LoggerMock { get; private set; }
        public ILog Logger => LoggerMock.Object;


        public Lazy<EmployerAccountsDbContext> LazyDbContext { get; }

        public EmployerAccountTeamRepository CreateRepository()
        {
            return new EmployerAccountTeamRepository(EmployerAccountsConfiguration, Logger, LazyDbContext);
        }

        public EmployerAccountTeamRepositoryTestFixtures WithNewAccountAndActiveUser(string email, Role role, out string hashedId)
        {
            using (var db = CreateDbContext())
            {
                hashedId = GetNextHashedIdForTests(db);
                var account = db.Accounts.Create();
                account.CreatedDate = DateTime.Now;
                account.HashedId = hashedId;
                account.Name = $"Account created for unit test {hashedId}";

                var user = db.Users.Create();
                user.FirstName = email;
                user.LastName = "User created for unit test";
                user.Email = email;

                var membership = db.Memberships.Create();
                membership.Account = account;
                membership.User = user;
                membership.Role = role;

                db.Accounts.Add(account);
                db.Users.Add(user);
                db.Memberships.Add(membership);
                db.SaveChanges();
            }

            return this;
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
