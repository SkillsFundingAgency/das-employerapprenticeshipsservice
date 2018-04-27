using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.NLog.Logger;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Web.UnitTests.Authorization
{
	[TestFixture]
    public class AuthorizationServiceTests
    {
        [Test]
        public void ValidateMembership_WhenLoggedInAsAccountOwner_ThenShouldNotThrowUnauthorizedAccessException()
        {
	        var fixtures = new AuthorizationTestFixtures()
							.WithAccountOwner(123, 456)
							.WithLoggedInAsAccount(123, 456);

	        var authorisationService = fixtures.CreateAuthorizationService();

			authorisationService.ValidateMembership();

			Assert.Pass();
        }

        [Test]
        public void ValidateMembership_WhenLoggedInAsAccountOwner_ThenShouldThrowUnauthorizedAccessExceptionIfMembershipIsInvalid()
        {
	        var fixtures = new AuthorizationTestFixtures();
	        var authorizationService = fixtures.CreateAuthorizationService();

            Assert.Throws<UnauthorizedAccessException>(() => authorizationService.ValidateMembership());
        }

	    [Test]
	    public void GetAuthorizationContext_WhenCalledWithValidLoggedInUser_ThenShouldReturnAuthorizationContext()
	    {
			// arrange
		    var fixtures = new AuthorizationTestFixtures()
			    .WithAccountOwner(123, 456)
			    .WithLoggedInAsAccount(123, 456);

		    var authorizationService = fixtures.CreateAuthorizationService();

			// Act
		    var authorizationContext =  authorizationService.GetAuthorizationContext();

			// Assert
		    var membership = fixtures.Memberships.Single();

		    Assert.That(authorizationContext, Is.Not.Null);
		    Assert.That(authorizationContext.AccountContext, Is.Not.Null);
		    Assert.That(authorizationContext.AccountContext.HashedId, Is.EqualTo(membership.Account.HashedId));
		    Assert.That(authorizationContext.AccountContext.Id, Is.EqualTo(membership.Account.Id));
		    Assert.That(authorizationContext.UserContext, Is.Not.Null);
		    Assert.That(authorizationContext.UserContext.ExternalId, Is.EqualTo(membership.User.ExternalId));
		    Assert.That(authorizationContext.UserContext.Id, Is.EqualTo(membership.User.Id));
		    Assert.That(authorizationContext.MembershipContext, Is.Not.Null);
		    Assert.That(authorizationContext.MembershipContext.Role, Is.EqualTo(membership.Role));
	    }


	    [Test]
	    public void GetAuthorizationContext_WhenGettingAuthorisationContext_ResultShouldBeCached()
	    {
		    // arrange
		    var fixtures = new AuthorizationTestFixtures()
			    .WithAccountOwner(123, 456)
			    .WithLoggedInAsAccount(123, 456);
			  
		    var authorizationService = fixtures.CreateAuthorizationService();

			// Act
		    authorizationService.GetAuthorizationContext();

			// Assert
		    fixtures.AuthorizationContextCache.Verify(cc => cc.SetAuthorizationContext(It.IsAny<AuthorizationContext>()));
	    }

	    [Test]
	    public void GetAuthorizationContext_WhenGettingAuthorisationContext_ShouldGetCachedVersion()
	    {
			// arrange
		    var expectedAuthorizationContext = new AuthorizationContext();

		    var fixtures = new AuthorizationTestFixtures()
			    .WithCachedAuthorizationContext(expectedAuthorizationContext);

		    var authorizationService = fixtures.CreateAuthorizationService();

			// Act
		    var actualAuthorizationContext = authorizationService.GetAuthorizationContext();

			// Assert
		    Assert.That(actualAuthorizationContext, Is.SameAs(expectedAuthorizationContext));

		    fixtures.Db.Verify(d => d.Memberships, Times.Never);
	    }

	    [Test]
	    public void GetAuthorizationContext_WhenUserIsNotAuthemticated_ThenShouldReturnNullUserContext()
	    {
			// Arrange
		    var fixtures = new AuthorizationTestFixtures();
		    var authorizationService = fixtures.CreateAuthorizationService();

			// Act
			var authorizationContext = authorizationService.GetAuthorizationContext();

			// Assert
		    Assert.That(authorizationContext.UserContext, Is.Null);
	    }

        [Test]
	    public void GetAuthorizationContext_ThenShouldReturnNullMembershipContextIfMembershipIsInvalid()
	    {
			// Arrange
		    var fixtures = new AuthorizationTestFixtures();
		    var authorizationService = fixtures.CreateAuthorizationService();

		    // Act
		    var authorizationContext = authorizationService.GetAuthorizationContext();

			// Assert
		    Assert.That(authorizationContext.MembershipContext, Is.Null);
	    }
	}

	class AuthorizationTestFixtures
	{
		public Mock<IAuthorizationContextCache> AuthorizationContextCache;
	    public Mock<ICallerContextProvider> CallerContextProvider;
        public Mock<EmployerAccountDbContext> Db;
		public Mock<IFeatureService> FeatureService;
		public Mock<ILog> Logger;
		public List<Mock<IAuthorizationHandler>> OperationAuthorisationHandlers;

		public IConfigurationProvider ConfigurationProvider;
		public DbSetStub<Account> AccountsDbSet;
		public List<Account> Accounts;

		public DbSetStub<User> UsersDbSet;
		public List<User> Users;

		public DbSetStub<Membership> MembershipsDbSet;
		public List<Membership> Memberships;

		public AuthorizationTestFixtures()
		{
			AuthorizationContextCache = new Mock<IAuthorizationContextCache>();
		    CallerContextProvider = new Mock<ICallerContextProvider>();
            Db = new Mock<EmployerAccountDbContext>();
			FeatureService = new Mock<IFeatureService>();
			Logger = new Mock<ILog>();
			OperationAuthorisationHandlers = new List<Mock<IAuthorizationHandler>>();

			Accounts = new List<Account>();
			AccountsDbSet = new DbSetStub<Account>(Accounts);

			Users = new List<User>();
			UsersDbSet = new DbSetStub<User>(Users);

			Memberships = new List<Membership>();
			MembershipsDbSet = new DbSetStub<Membership>(Memberships);

		    CallerContextProvider.Setup(a => a.GetCallerContext()).Returns(new CallerContext());

			Db.Setup(d => d.Accounts).Returns(AccountsDbSet);
			Db.Setup(d => d.Users).Returns(UsersDbSet);
			Db.Setup(d => d.Memberships).Returns(MembershipsDbSet);

			ConfigurationProvider = new MapperConfiguration(c =>
			{
				c.AddProfile<AccountMappings>();
				c.AddProfile<MembershipMappings>();
				c.AddProfile<UserMappings>();
			});

			QueryFutureManager.AllowQueryBatch = false;
		}

		public AuthorizationTestFixtures WithAccount(long accountId)
		{
			EnsureAccount(accountId);
			return this;
		}

		public AuthorizationTestFixtures WithUser(long userId)
		{
			EnsureUser(userId);
			return this;
		}

		public AuthorizationTestFixtures WithAccountOwner(long accountId, long userId)
		{
			var account = EnsureAccount(accountId);
			var user = EnsureUser(userId);

			var membership = new Membership
			{
				AccountId = accountId,
				UserId = userId,
				Account = account,
				User = user,
				Role = Role.Owner
			};

			Memberships.Add(membership);

			return this;
		}

		public AuthorizationTestFixtures WithLoggedInAsAccount(long accountId, long userId)
		{
			EnsureAccount(accountId);
			var user = EnsureUser(userId);

            CallerContextProvider.Setup(a => a.GetCallerContext()).Returns(new CallerContext
		    {
		        AccountId = accountId,
                UserExternalId = user.ExternalId
		    });

			return this;
		}

		public AuthorizationTestFixtures WithCachedAuthorizationContext(AuthorizationContext authorizationContext)
		{
			AuthorizationContextCache
				.Setup(cc => cc.GetAuthorizationContext())
				.Returns(authorizationContext);

			return this;
		}

		public AuthorizationService CreateAuthorizationService()
		{
			return new AuthorizationService(
				Db.Object,
                AuthorizationContextCache.Object,
				OperationAuthorisationHandlers.Select(mock => mock.Object).ToArray(),
				CallerContextProvider.Object,
                ConfigurationProvider,
				FeatureService.Object);
		}

		private Account EnsureAccount(long accountId)
		{
			var existingAccount = Accounts.SingleOrDefault(ac => ac.Id == accountId);

			if (existingAccount == null)
			{
				existingAccount = new Account
				{
					Id = accountId,
					HashedId = "ABC123"
				};

				Accounts.Add(existingAccount);
			}

			return existingAccount;
		}

		private User EnsureUser(long userId)
		{
			var existingUser = Users.SingleOrDefault(u => u.Id == userId);

			if (existingUser == null)
			{
				existingUser = new User
				{
					ExternalId = Guid.NewGuid(),
					Id = userId
				};

				Users.Add(existingUser);
			}

			return existingUser;
		}
	}
}