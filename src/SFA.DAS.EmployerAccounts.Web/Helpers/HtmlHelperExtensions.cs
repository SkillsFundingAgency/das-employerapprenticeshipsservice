using NLog;
using System.Web.Mvc;
using SFA.DAS.Authorization.Results;
using SFA.DAS.Authorization.Services;
using System.Security.Claims;
using System.Linq;
using MediatR;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Threading.Tasks;
using System.Web;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

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

        public static bool HasSignedV3AgreementAsync(this HtmlHelper htmlHelper)
        {
            var mediator = DependencyResolver.Current.GetService<IMediator>();
            var userId = htmlHelper.ViewContext.RequestContext.HttpContext.GetOwinContext().Authentication.
                User.Claims.FirstOrDefault(x => x.Type == ControllerConstants.UserRefClaimKeyName)?.Value;
            var hashedAccountId = htmlHelper.ViewContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName].ToString();
            var agreementResponse = Task.Run(async () => await mediator
            .SendAsync(new GetAccountEmployerAgreementsRequest
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = userId
            })).Result;

            if (agreementResponse.EmployerAgreements.Count > 0)
            {
                var employerAgreements = agreementResponse.EmployerAgreements;

                var legalEntityAgreements = employerAgreements.GroupBy(ea => ea.LegalEntity.AccountLegalEntityId);

                foreach (var legalEntityAgreement in legalEntityAgreements)
                {
                    var latestSignedAgreement = legalEntityAgreement
                        .Where(lea => lea.HasSignedAgreement)
                        .OrderByDescending(lea => lea.Signed.VersionNumber)
                        .FirstOrDefault();

                    if (latestSignedAgreement.Signed.VersionNumber != 3) return true;
                }
            }

            return false;


            //    foreach (var _ in from agreement in employerAgreements
            //                      where agreement.Signed?.VersionNumber == 3
            //                      select new { })
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }


        public static string ReturnToHomePageButtonHref(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isTier2User, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageButtonHref :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ");

            return isTier2User && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : isAccountIdSet ? $"/accounts/{accountId}/teams" : "/";
        }

        public static string ReturnToHomePageButtonText(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isTier2User, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageButtonText :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ");

            return isTier2User && isAccountIdSet ? "Return to your team" : isAccountIdSet ? "Go back to the account home page" : "Go back to the service home page";
        }

        public static string ReturnToHomePageLinkHref(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isTier2User, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageLinkHref :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ");

            return isTier2User && isAccountIdSet ? $"/accounts/{accountId}/teams/view" : "/";
        }

        public static string ReturnToHomePageLinkText(this HtmlHelper htmlHelper, string accountId)
        {
            accountId = GetHashedAccountId(htmlHelper, accountId, out bool isTier2User, out bool isAccountIdSet);

            Logger.Debug($"ReturnToHomePageLinkText :: Accountid : {accountId} IsTier2User : {isTier2User}  IsAccountIdSet : {isAccountIdSet} ");

            return isTier2User && isAccountIdSet ? "Back" : isAccountIdSet ? "Back to the homepage" : "Back";
        }

        private static string GetHashedAccountId(HtmlHelper htmlHelper, string accountId, out bool isTier2User, out bool isAccountIdSet)
        {
            isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;
            if (isTier2User && string.IsNullOrEmpty(accountId))
            {
                accountId = htmlHelper.ViewContext.RequestContext.HttpContext.User.Identity.HashedAccountId();
            }
            isAccountIdSet = !string.IsNullOrEmpty(accountId);
            return accountId;
        }

        public static string ReturnParagraphContent(this HtmlHelper htmlHelper)
        {
            bool isTier2User = htmlHelper.ViewContext.RequestContext.HttpContext.User?.IsInRole(Tier2User) ?? false;

            return isTier2User ? "You do not have permission to access this part of the service." : "If you are experiencing difficulty accessing the area of the site you need, first contact an/the account owner to ensure you have the correct role assigned to your account.";
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
            var controllerContext = html.ViewContext.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindView(controllerContext, viewName, null);

            return result.View != null;
        }
    }
}
