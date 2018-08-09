using Microsoft.Azure;
using SFA.DAS.EAS.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string CommitmentsAction(this UrlHelper helper, string path)
        {
            return Action(helper, path, ControllerConstants.CommitmentsBaseUrlKeyName);
        }

        public static string ProjectionsAction(this UrlHelper helper, string path)
        {
            return Action(helper, path, ControllerConstants.ProjectionsBaseUrlKeyName);
        }

        public static string RecruitmentsAction(this UrlHelper helper)
        {
            return Action(helper, string.Empty, ControllerConstants.RecruitmentsBaseUrlKeyName);
        }

        public static string EmployerAccountsAction(this UrlHelper helper, string path)
        {
            return Action(helper, path, ControllerConstants.EmployerAccountsWebBaseUrlKeyName);
        }

        public static string EmployerFinanceAction(this UrlHelper helper, string path)
        {
            return Action(helper, path, ControllerConstants.EmployerFinanceWebBaseUrlKeyName);
        }

        private static string Action(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];

            return $"{baseUrl}/accounts/{hashedAccountId}/{path}";
        }
    }
}