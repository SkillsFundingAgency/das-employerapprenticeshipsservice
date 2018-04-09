using System;
using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Services
{
	public interface ICallerContext
	{
		long? GetAccountId();
		Guid? GetUserExternalId();
		string GetControllerName();
		string GetOperationName();
		AuthorizationContext GetAuthorizationContext();
		void SetAuthorizationContext(AuthorizationContext context);
	}
}