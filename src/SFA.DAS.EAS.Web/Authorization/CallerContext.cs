using System;
using System.Web;
using SFA.DAS.EAS.Application.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.Authorization
{
	public class CallerContext : ICallerContext
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly HttpContextBase _httpContext;
		private readonly IHashingService _hashingService;

		public CallerContext(
			HttpContextBase httpContext,
			IAuthenticationService authenticationService,
			IHashingService hashingService)
		{
			_authenticationService = authenticationService;
			_httpContext = httpContext;
			_hashingService = hashingService;
		}

		public long? GetAccountId()
		{
			if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(ControllerConstants.AccountHashedIdRouteKeyName, out var accountHashedId))
			{
				return null;
			}

			if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out var accountId))
			{
				throw new UnauthorizedAccessException();
			}

			return accountId;
		}

		public Guid? GetUserExternalId()
		{
			if (!_authenticationService.IsUserAuthenticated())
			{
				return null;
			}

			if (!_authenticationService.TryGetClaimValue(ControllerConstants.UserExternalIdClaimKeyName, out var userExternalIdClaimValue))
			{
				throw new UnauthorizedAccessException();
			}

			if (!Guid.TryParse(userExternalIdClaimValue, out var userExternalId))
			{
				throw new UnauthorizedAccessException();
			}

			return userExternalId;
		}

		public string GetControllerName()
		{
			if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(ControllerConstants.ControllerKeyName, out var controllerName))
			{
				return null;
			}

			return (string)controllerName;
		}

		public string GetOperationName()
		{
			if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(ControllerConstants.ActionKeyName, out var operationName))
			{
				return null;
			}

			return (string)operationName;
		}
		private static readonly string Key = typeof(AuthorizationContext).FullName;

		public AuthorizationContext GetAuthorizationContext()
		{
			if (_httpContext.Items.Contains(Key))
			{
				return _httpContext.Items[Key] as AuthorizationContext;
			}

			return null;
		}

		public void SetAuthorizationContext(AuthorizationContext context)
		{
			_httpContext.Items[Key] = context;
		}
	}
}
