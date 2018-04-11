using System;
using System.Net.Http;
using SFA.DAS.EAS.Account.Api.Helpers;
using SFA.DAS.EAS.Application.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.Api.Authorization
{
    public class CallerContextProvider : ICallerContextProvider
    {
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly IHashingService _hashingService;

        public CallerContextProvider(HttpRequestMessage httpRequestMessage, IHashingService hashingService)
        {
            _httpRequestMessage = httpRequestMessage;
            _hashingService = hashingService;
        }

        public ICallerContext GetCallerContext()
        {
            var accountId = GetAccountId();

            return new CallerContext
            {
                AccountId = accountId,
                UserExternalId = null
            };
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