using System.Security.Claims;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SFA.DAS.EmployerAccounts.Helpers;
using SFA.DAS.EmployerAccounts.Queries.GetContent;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Helpers;

public interface IHtmlHelpers
{
    bool IsSupportUser();
    HtmlString GetContentByType(string type, bool useLegacyStyles = false);
    bool ViewExists(IHtmlHelper html, string viewName);
    string ReturnToHomePageButtonHref(string accountId);
    string ReturnToHomePageButtonText(string accountId);
    string ReturnToHomePageLinkHref(string accountId);
    string ReturnToHomePageLinkText(string accountId);
    string ReturnParagraphContent();
}

public class HtmlHelpers : IHtmlHelpers
{
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HtmlHelpers> _logger;
    private readonly ICompositeViewEngine _compositeViewEngine;


    public HtmlHelpers(
        EmployerAccountsConfiguration configuration,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<HtmlHelpers> logger,
        ICompositeViewEngine compositeViewEngine)
    {
        _configuration = configuration;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _compositeViewEngine = compositeViewEngine;
    }

    public bool IsSupportUser()
    {
        var requiredRoles = _configuration.SupportConsoleUsers.Split(',');
        if (!(_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity claimsIdentity) || !claimsIdentity.IsAuthenticated)
        {
            return false;
        }

        return requiredRoles.Any(role => claimsIdentity.Claims.Any(c => c.Type == claimsIdentity.RoleClaimType && c.Value.Equals(role)));
    }

    public static HtmlString SetZenDeskLabels(params string[] labels)
    {
        var keywords = string.Join(",", labels
            .Where(label => !string.IsNullOrEmpty(label))
            .Select(label => $"'{EscapeApostrophes(label)}'"));

        // when there are no keywords default to empty string to prevent zen desk matching articles from the url
        var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                            + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                            + "] });</script>";

        return new HtmlString(apiCallString);
    }

    private static string EscapeApostrophes(string input)
    {
        return input.Replace("'", @"\'");
    }

    public HtmlString GetContentByType(string type, bool useLegacyStyles = false)
    {
        var userResponse = AsyncHelper.RunSync(() => _mediator.Send(new GetContentRequest
        {
            UseLegacyStyles = useLegacyStyles,
            ContentType = type
        }));

        return new HtmlString(userResponse.Content);
    }

    private string GetHashedAccountId(string accountId, out bool isConsoleUser, out bool isAccountIdSet)
    {
        isConsoleUser = IsSupportConsoleUser();
        if (IsSupportConsoleUser() && string.IsNullOrEmpty(accountId))
        {
            accountId = _httpContextAccessor.HttpContext.User.Identity.HashedAccountId();
        }
        isAccountIdSet = !string.IsNullOrEmpty(accountId);
        return accountId;
    }

    public bool ViewExists(IHtmlHelper html, string viewName)
    {
        var result = _compositeViewEngine.FindView(html.ViewContext, viewName, false);

        return result.View != null;
    }

    private bool IsSupportConsoleUser()
    {
        var requiredRoles = _configuration.SupportConsoleUsers.Split(',');
        return requiredRoles.Any(role => _httpContextAccessor.HttpContext.User.IsInRole(role));
    }

    public string ReturnToHomePageButtonHref(string accountId)
    {
        accountId = GetHashedAccountId(accountId, out bool isConsoleUser, out bool isAccountIdSet);

        _logger.LogDebug("ReturnToHomePageButtonHref :: Accountid : {AccountId} IsConsoleUser : {IsConsoleUser}  IsAccountIdSet : {IsAccountIdSet} ", accountId, isConsoleUser, isAccountIdSet);

        return isConsoleUser && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : isAccountIdSet ? $"/accounts/{accountId}/teams" : "/";
    }

    public string ReturnToHomePageButtonText(string accountId)
    {
        accountId = GetHashedAccountId(accountId, out bool isConsoleUser, out bool isAccountIdSet);

        _logger.LogDebug("ReturnToHomePageButtonText :: Accountid : {AccountId} IsConsoleUser : {IsConsoleUser}  IsAccountIdSet : {IsAccountIdSet} ", accountId, isConsoleUser, isAccountIdSet);

        return isConsoleUser && isAccountIdSet ? "Return to your team" : isAccountIdSet ? "Go back to the account home page" : "Go back to the service home page";
    }

    public string ReturnToHomePageLinkHref(string accountId)
    {
        accountId = GetHashedAccountId(accountId, out bool isConsoleUser, out bool isAccountIdSet);

        _logger.LogDebug("ReturnToHomePageLinkHref :: Accountid : {AccountId} IsConsoleUser : {IsConsoleUser}  IsAccountIdSet : {IsAccountIdSet} ", accountId, isConsoleUser, isAccountIdSet);

        return isConsoleUser && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : "/";
    }

    public string ReturnToHomePageLinkText(string accountId)
    {
        accountId = GetHashedAccountId(accountId, out bool isConsoleUser, out bool isAccountIdSet);

        _logger.LogDebug("ReturnToHomePageLinkText :: Accountid : {AccountId} IsConsoleUser : {IsConsoleUser}  IsAccountIdSet : {IsAccountIdSet} ", accountId, isConsoleUser, isAccountIdSet);

        return isConsoleUser && isAccountIdSet ? "Back" : isAccountIdSet ? "Back to the homepage" : "Back";
    }

    public string ReturnParagraphContent()
    {
        return IsSupportConsoleUser() ? "You do not have permission to access this part of the service." : "If you are experiencing difficulty accessing the area of the site you need, first contact an/the account owner to ensure you have the correct role assigned to your account.";
    }
}
