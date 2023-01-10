using NLog;
using System.Web.Mvc;
using SFA.DAS.Authorization.Results;
using SFA.DAS.Authorization.Services;
using System.Security.Claims;
using System.Linq;
using MediatR;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
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

        public static bool ShowExpiringAgreementBanner(this HtmlHelper htmlHelper,string userId,string hashedAccountId)
        {
            var mediator = DependencyResolver.Current.GetService<IMediator>();
            var agreementResponse = Task.Run(async () => await mediator
            .SendAsync(new GetAccountEmployerAgreementsRequest
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = userId
            })).Result;

            if (agreementResponse.EmployerAgreements.Where(ea => ea.HasSignedAgreement).Count() > 0)
            {
                var employerAgreements = agreementResponse.EmployerAgreements;

                var legalEntityAgreements = employerAgreements.GroupBy(ea => ea.LegalEntity.AccountLegalEntityId);

                foreach (var legalEntityAgreement in legalEntityAgreements)
                {
                    var latestSignedAgreement = legalEntityAgreement
                        .Where(lea => lea.HasSignedAgreement)
                        .OrderByDescending(lea => lea.Signed.VersionNumber)
                        .FirstOrDefault();

                    if (latestSignedAgreement?.Signed.VersionNumber != 3) return true;
                }
            }
            return false;
        }


        public static string ReturnToHomePageButtonHref(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isConsoleUser, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageButtonHref :: Accountid : {accountId} IsConsoleUser : {isConsoleUser}  IsAccountIdSet : {isAccountIdSet} ");

            return isConsoleUser && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : isAccountIdSet ? $"/accounts/{accountId}/teams" : "/";
        }

        public static string ReturnToHomePageButtonText(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isConsoleUser, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageButtonText :: Accountid : {accountId} IsConsoleUser : {isConsoleUser}  IsAccountIdSet : {isAccountIdSet} ");

            return isConsoleUser && isAccountIdSet ? "Return to your team" : isAccountIdSet ? "Go back to the account home page" : "Go back to the service home page";
        }

        public static string ReturnToHomePageLinkHref(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isConsoleUser, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageLinkHref :: Accountid : {accountId} IsConsoleUser : {isConsoleUser}  IsAccountIdSet : {isAccountIdSet} ");

            return isConsoleUser && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : "/";
        }

        public static string ReturnToHomePageLinkText(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isConsoleUser, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageLinkText :: Accountid : {accountId} IsConsoleUser : {isConsoleUser}  IsAccountIdSet : {isAccountIdSet} ");

            return isConsoleUser && isAccountIdSet ? "Back" : isAccountIdSet ? "Back to the homepage" : "Back";
        }

        private static string GetHashedAccountId(HtmlHelper htmlHelper, string accountId, out bool isConsoleUser, out bool isAccountIdSet)
        {
            isConsoleUser = IsSupportConsoleUser(htmlHelper);
            if (IsSupportConsoleUser(htmlHelper) && string.IsNullOrEmpty(accountId))
            {
                accountId = htmlHelper.ViewContext.RequestContext.HttpContext.User.Identity.HashedAccountId();
            }
            isAccountIdSet = !string.IsNullOrEmpty(accountId);
            return accountId;
        }

        public static string ReturnParagraphContent(this HtmlHelper htmlHelper)
        {
            return IsSupportConsoleUser(htmlHelper) ? "You do not have permission to access this part of the service." : "If you are experiencing difficulty accessing the area of the site you need, first contact an/the account owner to ensure you have the correct role assigned to your account.";
        }

        public static string GetClaimsHashedAccountId(this HtmlHelper htmlHelper)
        {
            var identity = htmlHelper.ViewContext.RequestContext.HttpContext.User.Identity as ClaimsIdentity;
            var claim = identity?.Claims.FirstOrDefault(c => c.Type == RouteValueKeys.AccountHashedId);
            var hashedAccountId = claim?.Value;

            Logger.Debug($"GetClaimsHashedAccountId :: HashedAccountId : {hashedAccountId} ");
            return (!string.IsNullOrEmpty(hashedAccountId)) ? hashedAccountId : string.Empty;
        }

        public static bool ViewExists(this HtmlHelper html, string viewName)
        {
            var controllerContext = Microsoft.AspNetCore.Mvc.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindView(controllerContext, viewName, null);

            return result.View != null;
        }

        public static bool IsSupportConsoleUser(HtmlHelper htmlHelper)
        {
            var config = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var requiredRoles = config.SupportConsoleUsers.Split(',');
            return requiredRoles.Any(role => htmlHelper.ViewContext.RequestContext.HttpContext.User.IsInRole(role));
        }
    }
}
