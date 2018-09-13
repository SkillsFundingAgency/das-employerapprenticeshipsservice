using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EmployerFinance.Web.Helpers;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string CommitmentsAction(this UrlHelper helper, string path)
        {
            return CommitmentAction(helper, path);
        }

        public static string LegacyEasAccountAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path);
        }

        public static string LegacyEasAction(this UrlHelper helper, string path)
        {
            return LegacyEasAction(path);
        }

        private static string AccountAction(UrlHelper helper, string path)
        {
            var hashedAccountId =
                helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";
            return LegacyEasAction(accountPath);
        }

        private static string CommitmentAction(UrlHelper helper, string path)
        {
            var baseUrl = CloudConfigurationManager.GetSetting("CommitmentBaseUrl");
            var hashedAccountId =
                helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            return $"{baseUrl}/accounts/{hashedAccountId}/{path}";
        }

        private static string LegacyEasAction(string path)
        {
            var baseUrl = CloudConfigurationManager.GetSetting("LegacyEasWebsiteBaseUrl");
            return $"{baseUrl}/{path}";
        }
    }
}