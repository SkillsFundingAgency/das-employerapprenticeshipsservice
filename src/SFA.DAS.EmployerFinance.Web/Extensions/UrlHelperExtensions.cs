using Microsoft.Azure;
using SFA.DAS.EmployerFinance.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string CommitmentsAction(this UrlHelper helper, string path)
        {
            return Action(helper, path, ControllerConstants.CommitmentsBaseUrlKeyName);
        }

        public static string LegacyEasAccountAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.LegacyEasBaseUrlKeyName);
        }

        public static string LegacyEasAction(this UrlHelper helper, string path)
        {
            return Action(path, ControllerConstants.LegacyEasBaseUrlKeyName);
        }

        private static string AccountAction(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";

            return Action(accountPath, baseUrlKeyName);
        }
        private static string Action(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];

            return $"{baseUrl}/accounts/{hashedAccountId}/{path}";
        }
        private static string Action(string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');

            return $"{baseUrl}/{path}";
        }
    }
}