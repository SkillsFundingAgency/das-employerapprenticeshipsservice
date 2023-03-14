using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Models.UserAccounts;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.Authentication;

public interface IEmployerAccountAuthorisationHandler
{
    Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles);
    Task<bool> IsOutsideAccount(AuthorizationHandlerContext context);
}

public class EmployerAccountAuthorisationHandler : IEmployerAccountAuthorisationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccountService _accountsService;
    private readonly ILogger<EmployerAccountOwnerAuthorizationHandler> _logger;
    private readonly EmployerAccountsConfiguration _configuration;

    public EmployerAccountAuthorisationHandler(IHttpContextAccessor httpContextAccessor, IUserAccountService accountsService, ILogger<EmployerAccountOwnerAuthorizationHandler> logger, IOptions<EmployerAccountsConfiguration> configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountsService = accountsService;
        _logger = logger;
        _configuration = configuration.Value;
    }

    public async Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles)
    {
        if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.HashedAccountId))
        {
            return false;
        }
        var accountIdFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.HashedAccountId].ToString().ToUpper();
        var employerAccountClaim = context.User.FindFirst(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));

        if (employerAccountClaim?.Value == null)
            return false;

        Dictionary<string, EmployerUserAccountItem> employerAccounts;

        try
        {
            employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim.Value);
        }
        catch (JsonSerializationException e)
        {
            _logger.LogError(e, "Could not deserialize employer account claim for user");
            return false;
        }

        EmployerUserAccountItem employerIdentifier = null;

        if (employerAccounts != null)
        {
            employerIdentifier = employerAccounts.ContainsKey(accountIdFromUrl)
                ? employerAccounts[accountIdFromUrl] : null;
        }

        if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
        {
            var requiredIdClaim = _configuration.UseGovSignIn
                ? ClaimTypes.NameIdentifier : EmployerClaims.IdamsUserIdClaimTypeIdentifier;

            if (!context.User.HasClaim(c => c.Type.Equals(requiredIdClaim)))
                return false;

            var userClaim = context.User.Claims
                .First(c => c.Type.Equals(requiredIdClaim));

            var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;

            var userId = userClaim.Value;

            var result = await _accountsService.GetUserAccounts(userId, email);

            var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
            var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);

            var updatedEmployerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(associatedAccountsClaim.Value);

            userClaim.Subject.AddClaim(associatedAccountsClaim);

            if (!updatedEmployerAccounts.ContainsKey(accountIdFromUrl))
            {
                return false;
            }
            employerIdentifier = updatedEmployerAccounts[accountIdFromUrl];
        }

        if (!_httpContextAccessor.HttpContext.Items.ContainsKey(ContextItemKeys.EmployerIdentifier))
        {
            _httpContextAccessor.HttpContext.Items.Add(ContextItemKeys.EmployerIdentifier, employerAccounts.GetValueOrDefault(accountIdFromUrl));
        }

        if (!CheckUserRoleForAccess(employerIdentifier, allowAllUserRoles))
        {
            return false;
        }

        return true;
    }

    public Task<bool> IsOutsideAccount(AuthorizationHandlerContext context)
    {
        if (_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.HashedAccountId))
        {
            return Task.FromResult(false);
        }

        var requiredIdClaim = _configuration.UseGovSignIn ? ClaimTypes.NameIdentifier : EmployerClaims.IdamsUserIdClaimTypeIdentifier;

        if (!context.User.HasClaim(c => c.Type.Equals(requiredIdClaim)))
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    private static bool CheckUserRoleForAccess(EmployerUserAccountItem employerIdentifier, bool allowAllUserRoles)
    {
        if (!Enum.TryParse<EmployerUserRole>(employerIdentifier.Role, true, out var userRole))
        {
            return false;
        }

        return allowAllUserRoles || userRole == EmployerUserRole.Owner;
    }
}