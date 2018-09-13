using Microsoft.Azure;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string CommitmentsAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.CommitmentsBaseUrlKeyName);
        }

        public static string LegacyEasAccountAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.LegacyEasBaseUrlKeyName);
        }

        public static string LegacyEasAction(this UrlHelper helper, string path)
        {
            return Action(path, ControllerConstants.LegacyEasBaseUrlKeyName);
        }

        public static string ProjectionsAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.ProjectionsBaseUrlKeyName);
        }

        public static string RecruitmentsAction(this UrlHelper helper)
        {
            return AccountAction(helper, string.Empty, ControllerConstants.RecruitmentsBaseUrlKeyName);
        }

        private static string AccountAction(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";

            return Action(accountPath, baseUrlKeyName);
        }

        private static string Action(string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');

            return $"{baseUrl}/{path}";
        }
    }
}