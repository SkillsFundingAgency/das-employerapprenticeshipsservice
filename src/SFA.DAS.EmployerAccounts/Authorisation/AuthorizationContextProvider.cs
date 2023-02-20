using Microsoft.AspNetCore.Mvc.Infrastructure;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerFeatures.Context;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Authorisation;

//public class AuthorizationContextProvider : IAuthorizationContextProvider
//{
//    private readonly IEncodingService _encodingService;
//    private readonly IAuthenticationServiceWrapper _authenticationService;
//    private readonly IActionContextAccessor _actionContextAccessor;

//    public AuthorizationContextProvider(
//        IEncodingService encodingService,
//        IAuthenticationServiceWrapper authenticationService,
//        IActionContextAccessor actionContextAccessor)
//    {
//        _encodingService = encodingService;
//        _authenticationService = authenticationService;
//        _actionContextAccessor = actionContextAccessor;
//    }

//    public IAuthorizationContext GetAuthorizationContext()
//    {
//        var authorizationContext = new AuthorizationContext();
//        var accountValues = GetAccountValues();
//        var userValues = GetUserValues();

//        if (accountValues.Id.HasValue)
//        {
//            authorizationContext.AddEmployerUserRoleValues(accountValues.Id.Value, userValues.Ref.Value);
//        }

//        authorizationContext.AddEmployerFeatureValues(accountValues.Id, userValues.Email);

//        return authorizationContext;
//    }

//    private (string HashedId, long? Id) GetAccountValues()
//    {
//        if (!_actionContextAccessor.ActionContext.RouteData.Values.TryGetValue(RouteValues.EncodedAccountId, out var accountHashedId))
//        {
//            return (null, null);
//        }

//        if (!_encodingService.TryDecode(accountHashedId.ToString(), EncodingType.AccountId, out var accountId))
//        {
//            throw new UnauthorizedAccessException();
//        }

//        return (accountHashedId.ToString(), accountId);
//    }

//    private (Guid? Ref, string Email) GetUserValues()
//    {
//        if (!_authenticationService.IsUserAuthenticated())
//        {
//            return (null, null);
//        }

//        if (!_authenticationService.TryGetClaimValue(EmployerClaims.Id, out var userRefClaimValue))
//        {
//            throw new UnauthorizedAccessException();
//        }

//        if (!Guid.TryParse(userRefClaimValue, out var userRef))
//        {
//            throw new UnauthorizedAccessException();
//        }

//        if (!_authenticationService.TryGetClaimValue(EmployerClaims.Email, out var userEmail))
//        {
//            throw new UnauthorizedAccessException();
//        }

//        return (userRef, userEmail);
//    }

//    private static class EmployerClaims
//    {
//        public static readonly string Id = "http://das/employer/identity/claims/id";
//        public static readonly string Email = "http://das/employer/identity/claims/email_address";
//        public static readonly string Name = "http://das/employer/identity/claims/display_name";
//    }
//}