using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Account.Api.Helpers;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Authorization.AuthorizationServiceTests
{
    [TestFixture]
    public class WhenIGetAuthorizationContext
    {
        private IAuthorizationService _authorizationService;
        private Mock<EmployerAccountDbContext> _db;
        private HttpRequestMessage _httpRequestMessage;
        private IConfigurationProvider _configurationProvider;
        private Mock<IHashingService> _hashingService;
        private DbSetStub<Domain.Data.Entities.Account.Account> _accountsDbSet;
        private List<Domain.Data.Entities.Account.Account> _accounts;
        private Domain.Data.Entities.Account.Account _account;
        private IDictionary<string, object> _routeDataValues;
        private Mock<IHttpRouteData> _routeData;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();
            _httpRequestMessage = new HttpRequestMessage();

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
            });

            _hashingService = new Mock<IHashingService>();

            _account = new Domain.Data.Entities.Account.Account
            {
                Id = 111111,
                HashedId = "ABC123"
            };

            _accounts = new List<Domain.Data.Entities.Account.Account> { _account };
            _accountsDbSet = new DbSetStub<Domain.Data.Entities.Account.Account>(_accounts);
            _routeDataValues = new Dictionary<string, object> { [ControllerConstants.AccountHashedIdRouteKeyName] = _account.HashedId };
            _routeData = new Mock<IHttpRouteData>();
            
            _db.Setup(d => d.Accounts).Returns(_accountsDbSet);
            _routeData.Setup(d => d.Values).Returns(_routeDataValues);
            _httpRequestMessage.SetRequestContext(new HttpRequestContext { RouteData = _routeData.Object });
            _hashingService.Setup(h => h.DecodeValue(_account.HashedId)).Returns(_account.Id);

            _authorizationService = new AuthorizationService(_db.Object, _httpRequestMessage, _configurationProvider, _hashingService.Object);
        }

        [Test]
        public void ThenShouldReturnAuthorizationContext()
        {
            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext, Is.Not.Null);
            Assert.That(authorizationContext.AccountContext, Is.Not.Null);
            Assert.That(authorizationContext.AccountContext.HashedId, Is.EqualTo(_account.HashedId));
            Assert.That(authorizationContext.AccountContext.Id, Is.EqualTo(_account.Id));
        }

        [Test]
        public void ThenShouldAddAuthorizationContextToTheRequestCache()
        {
            var authorizationContext = _authorizationService.GetAuthorizationContext();
            var cachedAuthorizationContext = _httpRequestMessage.Properties[typeof(AuthorizationContext).FullName] as AuthorizationContext;

            Assert.That(cachedAuthorizationContext, Is.Not.Null);
            Assert.That(cachedAuthorizationContext, Is.SameAs(authorizationContext));
        }

        [Test]
        public void ThenShouldReturnAuthorizationContextFromTheRequestCache()
        {
            var cachedMembershipContext = new AuthorizationContext();

            _httpRequestMessage.Properties[typeof(AuthorizationContext).FullName] = cachedMembershipContext;

            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext, Is.SameAs(cachedMembershipContext));

            _hashingService.Verify(h => h.DecodeValue(_account.HashedId), Times.Never);
            _db.Verify(d => d.Memberships, Times.Never);
        }

        [Test]
        public void ThenShouldReturnNullAccountContextIfAccountHashedIdRouteValueCannotBeFound()
        {
            _routeDataValues.Clear();

            var authorizationContext = _authorizationService.GetAuthorizationContext();

            Assert.That(authorizationContext.AccountContext, Is.Null);
        }

        [Test]
        public void ThenShouldThrowUnauthorizedExceptionIfAccountHashedIdCannotBeDecoded()
        {
            _hashingService.Setup(h => h.DecodeValue(_account.HashedId)).Throws<Exception>();
            
            Assert.Throws<UnauthorizedAccessException>(() => _authorizationService.GetAuthorizationContext());
        }
    }
}