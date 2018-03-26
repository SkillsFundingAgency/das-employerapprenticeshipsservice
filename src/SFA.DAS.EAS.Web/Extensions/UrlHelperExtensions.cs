using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerCommitmentsAction(this UrlHelper helper, string path)
        {
            return EmployerAction(helper, path, ControllerConstants.EmployerCommitmentsBaseUrlKeyName);
        }

        public static string EmployerRecruitAction(this UrlHelper helper)
        {
            return EmployerAction(helper, string.Empty, ControllerConstants.EmployerRecruitBaseUrlKeyName);
        }

        public static string EmployerProjectionsAction(this UrlHelper helper, string path)
        {
            return EmployerAction(helper, path, ControllerConstants.EmployerProjectionsBaseUrlKeyName);
        }

        private static string EmployerAction(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];

            return $"{baseUrl}/accounts/{hashedAccountId}/{path}";
        }
    }
}