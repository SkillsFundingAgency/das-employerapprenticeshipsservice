using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.HashingService;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Web.UnitTests.Authorization.AuthorizationServiceTests
{
    [TestFixture]
    public class WhenIGetAuthorizationContext
    {
        private IAuthorizationService _authorizationService;
        private Mock<EmployerAccountDbContext> _db;
        private Mock<HttpContextBase> _httpContext;
        private Mock<IAuthenticationService> _authenticationService;
        private IConfigurationProvider _configurationProvider;
        private Mock<IHashingService> _hashingService;
        private DbSetStub<Account> _accountsDbSet;
        private List<Account> _accounts;
        private DbSetStub<User> _usersDbSet;
        private List<User> _users;
        private DbSetStub<Membership> _membershipsDbSet;
        private List<Membership> _memberships;
        private Membership _membership;
        private Account _account;
        private User _user;
        private IDictionary _items;
        private Mock<HttpRequestBase> _request;
        private RouteData _routeData;
        private string _userExternalIdClaimValue;

        [SetUp]
        public void Arrange()
        {
            QueryFutureManager.AllowQueryBatch = false;

            _db = new Mock<EmployerAccountDbContext>();
            _httpContext = new Mock<HttpContextBase>();
            _authenticationService = new Mock<IAuthenticationService>();

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<MembershipMappings>();
                c.AddProfile<UserMappings>();
            });

            _hashingService = new Mock<IHashingService>();

            _account = new Account
            {
                Id = 111111,
                HashedId = "ABC123"
            };

            _user = new User
            {
                Id = 222222,
                ExternalId = Guid.NewGuid()
            };

            _membership = new Membership
            {
                AccountId = _account.Id,
                UserId = _user.Id,
                Account = _account,
                User = _user,
                Role = Role.Owner
            };

            _accounts = new List<Account> { _account };
            _accountsDbSet = new DbSetStub<Account>(_accounts);
            _users = new List<User> { _user };
            _usersDbSet = new DbSetStub<User>(_users);
            _memberships = new List<Membership> { _membership };
            _membershipsDbSet = new DbSetStub<Membership>(_memberships);
            _items = new Dictionary<string, AuthorizationContext>();
            _request = new Mock<HttpRequestBase>();
            _routeData = new RouteData();
            _routeData.Values[ControllerConstants.AccountHashedIdRouteKeyName] = _account.HashedId;
            _userExternalIdClaimValue = _user.ExternalId.ToString();
            
            _db.Setup(d => d.Accounts).Returns(_accountsDbSet);
            _db.Setup(d => d.Users).Returns(_usersDbSet);
            _db.Setup(d => d.Memberships).Returns(_membershipsDbSet);
            _request.Setup(r => r.RequestContext).Returns(new RequestContext(_httpContext.Object, _routeData));
            _httpContext.Setup(c => c.Items).Returns(_items);
            _httpContext.Setup(c => c.Request).Returns(_request.Object);
            _authenticationService.Setup(a => a.IsUserAuthenticated()).Returns(true);
            _authenticationService.Setup(a => a.TryGetClaimValue(ControllerConstants.UserExternalIdClaimKeyName, out _userExternalIdClaimValue)).Returns(true);
            _hashingService.Setup(h => h.DecodeValue(_account.HashedId)).Returns(_account.Id);

            _authorizationService = new AuthorizationService(_db.Object, _httpContext.Object, _authenticationService.Object, _configurationProvider, _hashingService.Object);
        }

        [Test]
        public void ThenShouldReturnAuthorizationContext()
        {
            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext, Is.Not.Null);
            Assert.That(authorizationContext.AccountContext, Is.Not.Null);
            Assert.That(authorizationContext.AccountContext.HashedId, Is.EqualTo(_membership.Account.HashedId));
            Assert.That(authorizationContext.AccountContext.Id, Is.EqualTo(_membership.Account.Id));
            Assert.That(authorizationContext.UserContext, Is.Not.Null);
            Assert.That(authorizationContext.UserContext.ExternalId, Is.EqualTo(_membership.User.ExternalId));
            Assert.That(authorizationContext.UserContext.Id, Is.EqualTo(_membership.User.Id));
            Assert.That(authorizationContext.MembershipContext, Is.Not.Null);
            Assert.That(authorizationContext.MembershipContext.Role, Is.EqualTo(_membership.Role));
        }

        [Test]
        public void ThenShouldAddAuthorizationContextToTheRequestCache()
        {
            var authorizationContext = _authorizationService.GetAuthorizationContext();
            var cachedAuthorizationContext = _items[typeof(AuthorizationContext).FullName] as AuthorizationContext;

            Assert.That(cachedAuthorizationContext, Is.Not.Null);
            Assert.That(cachedAuthorizationContext, Is.SameAs(authorizationContext));
        }

        [Test]
        public void ThenShouldReturnAuthorizationContextFromTheRequestCache()
        {
            var cachedMembershipContext = new AuthorizationContext();

            _items[typeof(AuthorizationContext).FullName] = cachedMembershipContext;

            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext, Is.SameAs(cachedMembershipContext));

            _hashingService.Verify(h => h.DecodeValue(_account.HashedId), Times.Never);
            _db.Verify(d => d.Memberships, Times.Never);
        }

        [Test]
        public void ThenShouldReturnNullUserContextIfUserIsNotAuthenticated()
        {
            _authenticationService.Setup(a => a.IsUserAuthenticated()).Returns(false);

            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext.UserContext, Is.Null);
        }

        [Test]
        public void ThenShouldThrowUnauthorizedExceptionIfUserExternalIdClaimCannotBeFound()
        {
            _authenticationService.Setup(a => a.TryGetClaimValue(ControllerConstants.UserExternalIdClaimKeyName, out _userExternalIdClaimValue)).Returns(false);
            
            Assert.Throws<UnauthorizedAccessException>(() => _authorizationService.GetAuthorizationContext());
        }

        [Test]
        public void ThenShouldReturnNullAccountContextIfAccountHashedIdRouteValueCannotBeFound()
        {
            _routeData.Values.Clear();

            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext.AccountContext, Is.Null);
        }

        [Test]
        public void ThenShouldThrowUnauthorizedExceptionIfAccountHashedIdCannotBeDecoded()
        {
            _hashingService.Setup(h => h.DecodeValue(_account.HashedId)).Throws<Exception>();
            
            Assert.Throws<UnauthorizedAccessException>(() => _authorizationService.GetAuthorizationContext());
        }

        [Test]
        public void ThenShouldReturnNullMembershipContextIfMembershipIsInvalid()
        {
            _memberships.Remove(_membership);

            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext.MembershipContext, Is.Null);
        }
    }
}