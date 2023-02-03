using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.EmployerUsers;
using SFA.DAS.EmployerAccounts.Infrastructure;

namespace SFA.DAS.EmployerAccounts.Web.Authentication;

public interface IEmployerAccountAuthorisationHandler
{
    bool IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles);
}

public class EmployerAccountAuthorizationHandler : AuthorizationHandler<EmployerAccountRequirement>, IEmployerAccountAuthorisationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmployerAccountService _accountsService;
    private readonly ILogger<EmployerAccountAuthorizationHandler> _logger;
    private readonly EmployerAccountsConfiguration _configuration;

    public EmployerAccountAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IEmployerAccountService accountsService, ILogger<EmployerAccountAuthorizationHandler> logger, IOptions<EmployerAccountsConfiguration> configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountsService = accountsService;
        _logger = logger;
        _configuration = configuration.Value;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
    {
        if (!IsEmployerAuthorised(context, false))
        {
            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }

    public bool IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles)
    {
        if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValues.EmployerAccountId))
        {
            return false;
        }
        var accountIdFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValues.EmployerAccountId].ToString().ToUpper();
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
            _logger.LogError(e, "Could not deserialize employer account claim for user {claim}", employerAccountClaim.Value);
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

            var userClaim = context.User.Claims.First(c => c.Type.Equals(requiredIdClaim));
            var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
            
            var userId = userClaim.Value;

            var result = _accountsService.GetUserAccounts(userId, email).Result;

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

    private static bool CheckUserRoleForAccess(EmployerUserAccountItem employerIdentifier, bool allowAllUserRoles)
    {
        if (!Enum.TryParse<Role>(employerIdentifier.Role, true, out var userRole))
        {
            return false;
        }

        return allowAllUserRoles || userRole == Role.Owner;
    }
}