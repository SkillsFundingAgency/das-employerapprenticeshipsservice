using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EmployerFinance.Web.Helpers;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.EmployerAccountsBaseUrlKeyName);
        }

        public static string EmployerCommitmentsAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.EmployerCommitmentsBaseUrlKeyName);
        }

        public static string EmployerProjectionsAction(this UrlHelper helper, string path)
        {
            return AccountAction(helper, path, ControllerConstants.EmployerProjectionsBaseUrlKeyName);
        }

        public static string EmployerRecruitAction(this UrlHelper helper)
        {
            return AccountAction(helper, string.Empty, ControllerConstants.EmployerRecruitBaseUrlKeyName);
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

        private static string Action(string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');

            return $"{baseUrl}/{path}";
        }
    }
}