using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.RouteValues;
using EmployerClaims = SFA.DAS.GovUK.Auth.Employer.EmployerClaims;
using EmployerUserAccountItem = SFA.DAS.GovUK.Auth.Employer.EmployerUserAccountItem;

namespace SFA.DAS.EAS.Web.Authentication;

public interface IEmployerAccountAuthorisationHandler
{
    Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles);
    Task<bool> IsOutsideAccount(AuthorizationHandlerContext context);
}

public class EmployerAccountAuthorisationHandler : IEmployerAccountAuthorisationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAssociatedAccountsService _associatedAccountsService;
    private readonly ILogger<EmployerAccountAuthorisationHandler> _logger;
    private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

    public EmployerAccountAuthorisationHandler(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<EmployerAccountAuthorisationHandler> logger,
        IOptions<EmployerApprenticeshipsServiceConfiguration> configuration, 
        IAssociatedAccountsService associatedAccountsService)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _associatedAccountsService = associatedAccountsService;
        _configuration = configuration.Value;
    }

    public async Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context, bool allowAllUserRoles)
    {
        if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.HashedAccountId))
        {
            return false;
        }
        
        var accountIdFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.HashedAccountId].ToString().ToUpper();
        
        Dictionary<string, EmployerUserAccountItem> employerAccounts;

        try
        {
            employerAccounts = await _associatedAccountsService.GetAccounts(forceRefresh: false);
        }
        catch (JsonSerializationException e)
        {
            _logger.LogError(e, "Could not deserialize employer account claim for user");
            return false;
        }

        EmployerUserAccountItem employerIdentifier = null;

        if (employerAccounts != null)
        {
            employerIdentifier = employerAccounts.TryGetValue(accountIdFromUrl, out var account)
                ? account
                : null;
        }

        if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
        {
            if (!context.User.HasClaim(c => c.Type.Equals(ClaimTypes.NameIdentifier)))
            {
                return false;
            }

            var updatedEmployerAccounts = await _associatedAccountsService.GetAccounts(forceRefresh: true);

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

        return CheckUserRoleForAccess(employerIdentifier, allowAllUserRoles);
    }

    public Task<bool> IsOutsideAccount(AuthorizationHandlerContext context)
    {
        if (_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.HashedAccountId))
        {
            return Task.FromResult(false);
        }

        var requiredIdClaim = _configuration.UseGovSignIn ? ClaimTypes.NameIdentifier : EmployerClaims.IdamsUserIdClaimTypeIdentifier;

        return Task.FromResult(context.User.HasClaim(c => c.Type.Equals(requiredIdClaim)));
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