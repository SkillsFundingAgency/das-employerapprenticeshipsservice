using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.Testing.Helpers;

namespace SFA.DAS.EmployerAccounts.IntegrationTests.Data;

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

internal class EmployerAccountTeamRepositoryTestFixtures
{
    public EmployerAccountTeamRepositoryTestFixtures()
    {
        EmployerAccountsConfiguration = ConfigurationTestHelper.GetConfiguration<EmployerAccountsConfiguration>(ConfigurationKeys.EmployerAccounts);

        InvitationRepositoryLoggerMock = new Mock<ILogger<InvitationRepository>>();
        EmployerAccountTeamRepositoryLoggerMock = new Mock<ILogger<EmployerAccountTeamRepository>>();
    }

    public EmployerAccountsConfiguration EmployerAccountsConfiguration { get; }

    public Mock<ILogger<InvitationRepository>> InvitationRepositoryLoggerMock { get; private set; }
    public ILogger<InvitationRepository> InvitationRepositoryLogger => InvitationRepositoryLoggerMock.Object;

    public Mock<ILogger<EmployerAccountTeamRepository>> EmployerAccountTeamRepositoryLoggerMock { get; private set; }
    public ILogger<EmployerAccountTeamRepository> EmployerAccountTeamRepositoryLogger => EmployerAccountTeamRepositoryLoggerMock.Object;


    public EmployerAccountTeamRepositoryTestFixtures WithNewAccountAndActiveUser(string email, Role role, out string hashedId)
    {
        using (var db = CreateDbContext())
        {
            var account = CreateAccount(db);
            hashedId = account.HashedId;

            var user = new User
            {
                FirstName = email,
                LastName = "User created for unit test",
                Email = email,
                Ref = Guid.NewGuid()
            };

            var membership = new Membership
            {
                Account = account,
                User = user,
                Role = role,
                CreatedDate = DateTime.Now
            };

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
            repositoryCreator: db => new EmployerAccountTeamRepository(new Lazy<EmployerAccountsDbContext>(() => db)),
            action: action);
    }

    public Task CheckInvitationRepository(Func<InvitationRepository, Task> action)
    {
        return RunWithTransaction(
            repositoryCreator: db => new InvitationRepository(new Lazy<EmployerAccountsDbContext>(() => db)),
            action: action);
    }

    private async Task RunWithTransaction<TRepository>(
        Func<EmployerAccountsDbContext, TRepository> repositoryCreator,
        Func<TRepository, Task> action)
    {
        await using var db = CreateDbContext();
        await db.Database.BeginTransactionAsync();

        try
        {
            var repo = repositoryCreator(db);
            await action(repo);

            await db.Database.CurrentTransaction.CommitAsync();
        }
        catch (Exception)
        {
            await db.Database.CurrentTransaction.RollbackAsync();
            throw;
        }
    }

    private static Account CreateAccount(EmployerAccountsDbContext db)
    {
        var hashedId = GetNextHashedIdForTests(db);
        var account = new Account
        {
            CreatedDate = DateTime.Now,
            HashedId = hashedId,
            Name = $"Account created for unit test {hashedId}"
        };
        db.Accounts.Add(account);

        return account;
    }

    private static string GetNextHashedIdForTests(EmployerAccountsDbContext dbContext)
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

        var intPart = int.Parse(maxHashedId.Substring(2, 4)) + 1;
        return $"ZZ{intPart:D4}";
    }

    private EmployerAccountsDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>();
        optionsBuilder.UseSqlServer(EmployerAccountsConfiguration.DatabaseConnectionString);
        return new EmployerAccountsDbContext(optionsBuilder.Options);
    }
}