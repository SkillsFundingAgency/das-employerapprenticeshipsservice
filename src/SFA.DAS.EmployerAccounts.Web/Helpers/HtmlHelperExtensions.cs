using System.Web;
using NLog;
using System.Web.Mvc;
using SFA.DAS.Authorization.Results;
using SFA.DAS.Authorization.Services;
using System.Security.Claims;
using System.Linq;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public const string Tier2User = "Tier2User";
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();        

        public static AuthorizationResult GetAuthorizationResult(this HtmlHelper htmlHelper, string featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var authorizationResult = authorisationService.GetAuthorizationResult(featureType);

            return authorizationResult;
        }

        public static bool IsAuthorized(this HtmlHelper htmlHelper, string featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var isAuthorized = authorisationService.IsAuthorized(featureType);

            return isAuthorized;
        }

        public static string ReturnToHomePageButtonHref(this HtmlHelper htmlHelper, string accountId)
        {                    
            bool isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;
            if (isTier2User && string.IsNullOrEmpty(accountId))
            {
                accountId = GetContextAccountId();
            }
            bool isAccountIdSet = !string.IsNullOrEmpty(accountId);
            Logger.Debug($"ReturnToHomePageButtonHref :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ClaimsIdentity : { HttpContext.Current.User.Identity as ClaimsIdentity} ");
            return isTier2User && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : isAccountIdSet ? $"/accounts/{accountId}/teams" : "/";
        }        

        public static string ReturnToHomePageButtonText(this HtmlHelper htmlHelper, string accountId)
        {
            bool isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;
            if (isTier2User && string.IsNullOrEmpty(accountId))
            {
                accountId = GetContextAccountId();
            }
            bool isAccountIdSet = !string.IsNullOrEmpty(accountId);
            Logger.Debug($"ReturnToHomePageButtonText :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ClaimsIdentity : { HttpContext.Current.User.Identity as ClaimsIdentity} ");

            return isTier2User && isAccountIdSet ? "Return to your team" : isAccountIdSet ? "Go back to the account home page" : "Go back to the service home page";
        }

        public static string ReturnToHomePageLinkHref(this HtmlHelper htmlHelper, string accountId)
        {
            bool isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;
            if (isTier2User && string.IsNullOrEmpty(accountId))
            {
                accountId = GetContextAccountId();
            }
            bool isAccountIdSet = !string.IsNullOrEmpty(accountId);
            Logger.Debug($"ReturnToHomePageLinkHref :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ClaimsIdentity : { HttpContext.Current.User.Identity as ClaimsIdentity} ");

            return isTier2User && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : "/";
        }

        public static string ReturnToHomePageLinkText(this HtmlHelper htmlHelper, string accountId)
        {           
            bool isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;
            if (isTier2User && string.IsNullOrEmpty(accountId))
            {
                accountId = GetContextAccountId();
            }
            bool isAccountIdSet = !string.IsNullOrEmpty(accountId);
            Logger.Debug($"ReturnToHomePageLinkText :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ClaimsIdentity : { HttpContext.Current.User.Identity as ClaimsIdentity} ");

            return isTier2User && isAccountIdSet ? "Back" : isAccountIdSet ? "Back to the homepage" : "Back";
        }

        public static string ReturnParagraphContent(this HtmlHelper htmlHelper)
        {
            bool isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;

            return isTier2User ? "You do not have permission to access this part of the service." : "If you are experiencing difficulty accessing the area of the site you need, first contact an/the account owner to ensure you have the correct role assigned to your account.";
        }

        public static string GetClaimsHashedAccountId()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            var claim = identity?.Claims.FirstOrDefault(c => c.Type == RouteValueKeys.AccountHashedId);
            var hashedAccountId = claim?.Value;
            Logger.Debug($"GetClaimsHashedAccountId :: HashedAccountId : {hashedAccountId} ");
            return (!string.IsNullOrEmpty(hashedAccountId)) ? hashedAccountId : string.Empty;
        }

        public static string GetContextAccountId()
        {
            string contextAccountId = string.Empty;
            string[] url = HttpContext.Current.Request.RawUrl.Split('/');
            if (url != null && url.Length > 2)
            {
                if (url[2] != null) { contextAccountId = url[2]; }
            }

            return contextAccountId;
        }

        public static bool ViewExists(this HtmlHelper html, string viewName)
        {
            var controllerContext = html.ViewContext.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindView(controllerContext, viewName, null);

            return result.View != null;
        }


        public static MvcHtmlString SetZenDeskLabels(this HtmlHelper html, params string[] labels)
        {
            var apiCallString =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!first) apiCallString += ",";
                first = false;

                apiCallString += $"'{label}'";
            }
            
            apiCallString += "] });</script>";

            return MvcHtmlString.Create(apiCallString);
        }
    }

    public static class ZenDeskLabels
    {
        public static string RegisterForAnApprenticeshipServiceAccount = "Register for an apprenticeship service account";
        public static string AddPAYESchemesToYourAccount = "Add PAYE schemes to your account";
        public static string ApprenticeshipFunding = "Apprenticeship funding";
        public static string EnteredTheWrongPAYESchemeDetails = "Entered the wrong PAYE scheme details";
        public static string UseYourGovernmentGatewayDetails = "Use your Government Gateway details";
        public static string ReviewTheEmployerAgreement = "Review the employer agreement";
    }
}
