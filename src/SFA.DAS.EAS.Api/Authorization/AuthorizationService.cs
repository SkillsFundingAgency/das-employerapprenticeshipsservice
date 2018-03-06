using System;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Account.Api.Helpers;
using SFA.DAS.EAS.Application.Extensions;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.Api.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private static readonly string Key = typeof(AuthorizationContext).FullName;

        private readonly EmployerAccountDbContext _db;
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IHashingService _hashingService;

        public AuthorizationService(EmployerAccountDbContext db, HttpRequestMessage httpRequestMessage, IConfigurationProvider configurationProvider, IHashingService hashingService)
        {
            _db = db;
            _httpRequestMessage = httpRequestMessage;
            _configurationProvider = configurationProvider;
            _hashingService = hashingService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            if (_httpRequestMessage.Properties.ContainsKey(Key))
            {
                return _httpRequestMessage.Properties[Key] as AuthorizationContext;
            }

            var accountId = GetAccountId();

            var accountContext = accountId == null ? null : _db.Accounts
                .Where(a => a.Id == accountId.Value)
                .ProjectTo<AccountContext>(_configurationProvider)
                .SingleOrDefault();

            var authorizationContext = new AuthorizationContext
            {
                AccountContext = accountContext
            };

            _httpRequestMessage.Properties[Key] = authorizationContext;

            return authorizationContext;
        }

        private long? GetAccountId()
        {
            if (!_httpRequestMessage.GetRouteData().Values.TryGetValue(ControllerConstants.AccountHashedIdRouteKeyName, out var accountHashedId))
            {
                return null;
            }

            if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out var accountId))
            {
                throw new UnauthorizedAccessException();
            }

            return accountId;
        }
    }
}