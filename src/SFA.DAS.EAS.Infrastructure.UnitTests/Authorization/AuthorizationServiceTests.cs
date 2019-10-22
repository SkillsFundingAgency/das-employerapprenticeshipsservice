﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.TestCommon;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Authorization
{
    [TestFixture]
    public class AuthorizationServiceTests : FluentTest<AuthorizationServiceTestFixture>
    {
        [Test]
        public void GetAuthorizationContext_WhenCurrentUserIsAMember_ThenShouldReturnAuthorizationContext()
        {
            Run(f => f.SetMember(1, 2).SetCurrentUser(1, 2), f => f.GetAuthorizationContext(), (f, r) => r.Should().NotBeNull()
                .And.Match<IAuthorizationContext>(c => 
                    c.AccountContext != null &&
                    c.AccountContext.Id == 2 &&
                    c.UserContext != null &&
                    c.UserContext.Id == 1 &&
                    c.MembershipContext != null));
        }

        [Test]
        public void GetAuthorizationContext_WhenCurrentUserIsAuthenticated_ThenShouldReturnAuthorizationContext()
        {
            Run(f => f.SetCurrentUser(1), f => f.GetAuthorizationContext(), (f, r) => r.Should().NotBeNull()
                .And.Match<IAuthorizationContext>(c =>
                    c.AccountContext == null &&
                    c.UserContext != null &&
                    c.UserContext.Id == 1 &&
                    c.MembershipContext == null));
        }

        [Test]
        public void GetAuthorizationContext_WhenCurrentUserIsUnauthenticated_ThenShouldReturnAuthorizationContext()
        {
            Run(f => f.GetAuthorizationContext(), (f, r) => r.Should().NotBeNull()
                .And.Match<IAuthorizationContext>(c =>
                    c.AccountContext == null &&
                    c.UserContext == null &&
                    c.MembershipContext == null));
        }

        [Test]
        public void GetAuthorizationContext_WhenCurrentUserIsNotAMember_ThenShouldReturnAuthorizationContext()
        {
            Run(f => f.SetCurrentUser(1, 2), f => f.GetAuthorizationContext(), (f, r) => r.ShouldThrow<UnauthorizedAccessException>());
        }

        [TestCase(true)]
        [TestCase(true, AuthorizationResult.Ok)]
        [TestCase(true, AuthorizationResult.Ok, AuthorizationResult.Ok)]
        [TestCase(false, AuthorizationResult.FeatureDisabled)]
        [TestCase(false, AuthorizationResult.Ok, AuthorizationResult.FeatureAgreementNotSigned)]
        [TestCase(false, AuthorizationResult.Ok, AuthorizationResult.FeatureUserNotWhitelisted, AuthorizationResult.Ok)]
        public void IsAuthorized_WhenICheckIfOperationIsAuthorized_ThenShouldReturnExpectedResult(bool expectedResult, params AuthorizationResult[] handlerResults)
        {
            Run(f => f.SetHandlerResults(handlerResults), f => f.IsAuthorized(), (f, r) => r.Should().Be(expectedResult));
        }
        
        [Test]
        public void ValidateMembership_WhenCurrentUserIsAMember_ThenShouldNotThrowUnauthorizedAccessException()
        {
            Run(f => f.SetMember(1, 2).SetCurrentUser(1, 2), f => f.ValidateMembership(), f => Assert.Pass());
        }
        
        [Test]
        public void ValidateMembership_WhenCurrentUserIsUnauthenticated_ThenShouldThrowUnauthorizedAccessException()
        {
            Run(f => f.ValidateMembership(), (f, r) => r.ShouldThrow<UnauthorizedAccessException>());
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(1, 2, 3, 2)]
        [TestCase(1, 2, 1, 3)]
        public void ValidateMembership_WhenCurrentUserIsNotAMember_ThenShouldThrowUnauthorizedAccessException(int memberUserId, int membershipAccountId, int currentUserId, int currentAccountId)
        {
            Run(f => f.SetMember(memberUserId, membershipAccountId).SetCurrentUser(currentUserId, currentAccountId), f => f.ValidateMembership(), (f, r) => r.ShouldThrow<UnauthorizedAccessException>());
        }
    }

    public class AuthorizationServiceTestFixture : FluentTestFixture
    {
        public List<Domain.Models.Account.Account> Accounts { get; }
        public DbSetStub<Domain.Models.Account.Account> AccountsDbSet { get; }
        public Mock<IAuthorizationContextCache> AuthorizationContextCache { get; }
        public Mock<ICallerContextProvider> CallerContextProvider { get; }
        public IConfigurationProvider ConfigurationProvider { get; }
        public Mock<EmployerAccountsDbContext> Db { get; }
        public Feature Feature { get; }
        public Mock<IFeatureService> FeatureService { get; }
        public List<IAuthorizationHandler> Handlers { get; }
        public List<Membership> Memberships { get; set; }
        public DbSetStub<Membership> MembershipsDbSet { get; }
        public List<User> Users { get; }
        public DbSetStub<User> UsersDbSet { get; }

        public AuthorizationServiceTestFixture()
        {
            Feature = new Feature { Enabled = true, FeatureType = FeatureType.Test1 };
            Db = new Mock<EmployerAccountsDbContext>();
            AuthorizationContextCache = new Mock<IAuthorizationContextCache>();
            Handlers = new List<IAuthorizationHandler>();
            CallerContextProvider = new Mock<ICallerContextProvider>();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<MembershipMappings>();
                c.AddProfile<UserMappings>();
            });

            FeatureService = new Mock<IFeatureService>();
            Accounts = new List<Domain.Models.Account.Account>();
            AccountsDbSet = new DbSetStub<Domain.Models.Account.Account>(Accounts);
            Users = new List<User>();
            UsersDbSet = new DbSetStub<User>(Users);
            Memberships = new List<Membership>();
            MembershipsDbSet = new DbSetStub<Membership>(Memberships);

            Db.Setup(d => d.Accounts).Returns(AccountsDbSet);
            Db.Setup(d => d.Users).Returns(UsersDbSet);
            Db.Setup(d => d.Memberships).Returns(MembershipsDbSet);
            CallerContextProvider.Setup(c => c.GetCallerContext()).Returns(new CallerContext());
            FeatureService.Setup(f => f.GetFeature(Feature.FeatureType)).Returns(Feature);

            QueryFutureManager.AllowQueryBatch = false;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationService = new AuthorizationService(AuthorizationContextCache.Object,
                Handlers,
                CallerContextProvider.Object,
                ConfigurationProvider,
                FeatureService.Object,
                new Lazy<EmployerAccountsDbContext>(() => Db.Object));

            return authorizationService.GetAuthorizationContext();
        }

        public bool IsAuthorized()
        {
            var authorizationService = new AuthorizationService(AuthorizationContextCache.Object,
                Handlers,
                CallerContextProvider.Object,
                ConfigurationProvider,
                FeatureService.Object,
                new Lazy<EmployerAccountsDbContext>(() => Db.Object));

            return authorizationService.IsAuthorized(Feature.FeatureType);
        }

        public AuthorizationServiceTestFixture SetHandlerResults(params AuthorizationResult[] handlerResults)
        {
            foreach (var handlerResult in handlerResults)
            {
                var handler = new Mock<IAuthorizationHandler>();

                handler.Setup(h => h.CanAccessAsync(It.IsAny<IAuthorizationContext>(), Feature)).ReturnsAsync(handlerResult);

                Handlers.Add(handler.Object);
            }

            return this;
        }

        public AuthorizationServiceTestFixture SetMember(int userId, int accountId)
        {
            var user = EnsureUser(userId);
            var account = EnsureAccount(accountId);

            var membership = new Membership
            {
                Account = account,
                User = user,
                Role = Role.Owner
            };
            
            Memberships.Add(membership);

            return this;
        }

        public AuthorizationServiceTestFixture SetCurrentUser(int userId, int? accountId = null)
        {
            var user = EnsureUser(userId);
            var account = accountId == null ? null : EnsureAccount(accountId.Value);

            CallerContextProvider.Setup(p => p.GetCallerContext()).Returns(new CallerContext
            {
                AccountId = account?.Id,
                UserRef = user.Ref
            });

            return this;
        }

        public void ValidateMembership()
        {
            var authorizationService = new AuthorizationService(AuthorizationContextCache.Object,
                Handlers,
                CallerContextProvider.Object,
                ConfigurationProvider,
                FeatureService.Object,
                new Lazy<EmployerAccountsDbContext>(() => Db.Object));

            authorizationService.ValidateMembership();
        }

        private Domain.Models.Account.Account EnsureAccount(int accountId)
        {
            var existingAccount = Accounts.SingleOrDefault(ac => ac.Id == accountId);

            if (existingAccount == null)
            {
                existingAccount = new Domain.Models.Account.Account
                {
                    Id = accountId
                };

                Accounts.Add(existingAccount);
            }

            return existingAccount;
        }

        private User EnsureUser(int userId)
        {
            var existingUser = Users.SingleOrDefault(u => u.Id == userId);

            if (existingUser == null)
            {
                existingUser = new User
                {
                    Id = userId,
                    Ref = Guid.NewGuid()
                };

                Users.Add(existingUser);
            }

            return existingUser;
        }
    }
}