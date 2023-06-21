using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.RouteValues;

namespace SFA.DAS.EAS.Support.Web.Authentication;

public interface IEmployerAccountAuthorisationHandler
{
    Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles);
    bool CheckUserAccountAccess(ClaimsPrincipal user, EmployerUserRole userRoleRequired);
}

public class EmployerAccountAuthorisationHandler : IEmployerAccountAuthorisationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccountService _accountsService;
    private readonly ILogger<EmployerAccountAuthorisationHandler> _logger;
    private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

    public EmployerAccountAuthorisationHandler(IHttpContextAccessor httpContextAccessor, IUserAccountService accountsService, ILogger<EmployerAccountAuthorisationHandler> logger, IOptions<EmployerApprenticeshipsServiceConfiguration> configuration)
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

    public bool CheckUserAccountAccess(ClaimsPrincipal user, EmployerUserRole userRoleRequired)
    {
        if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.HashedAccountId))
        {
            return false;
        }

        Dictionary<string, EmployerUserAccountItem> employerAccounts;
        var accountIdFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.HashedAccountId].ToString().ToUpper();
        var employerAccountClaim = user.FindFirst(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));
        try
        {
            employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim.Value);
        }
        catch (JsonSerializationException e)
        {
            _logger.LogError(e, "Could not deserialize employer account claim for user");
            return false;
        }

        if (employerAccounts == null)
        {
            return false;
        }

        var employerIdentifier = employerAccounts.ContainsKey(accountIdFromUrl)
            ? employerAccounts[accountIdFromUrl] : null;

        if (employerIdentifier == null)
        {
            return false;
        }

        if (!Enum.TryParse<EmployerUserRole>(employerIdentifier.Role, true, out var claimUserRole))
        {
            return false;
        }

        switch (userRoleRequired)
        {
            case EmployerUserRole.Owner when claimUserRole == EmployerUserRole.Owner:
            case EmployerUserRole.Transactor when claimUserRole is EmployerUserRole.Owner or EmployerUserRole.Transactor:
            case EmployerUserRole.Viewer when claimUserRole is EmployerUserRole.Owner or EmployerUserRole.Transactor or EmployerUserRole.Viewer:
                return true;
            default:
                return false;
        }
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