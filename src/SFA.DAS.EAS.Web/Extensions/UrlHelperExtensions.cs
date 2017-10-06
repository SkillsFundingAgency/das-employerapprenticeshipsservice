using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ExternalUrlAction(this UrlHelper helper, string controllerName, string actionName = "")
        {

            var baseUrl = GetBaseUrl();

            var accountId = helper.RequestContext.RouteData.Values[ControllerConstants.HashedAccountIdKeyName];

            return $"{baseUrl}accounts/{accountId}/{controllerName}/{actionName}";
        }

        private static string GetBaseUrl()
        {
            return CloudConfigurationManager.GetSetting(ControllerConstants.EmployerCommitmentsBaseUrlKeyName).EndsWith("/")
                ? CloudConfigurationManager.GetSetting(ControllerConstants.EmployerCommitmentsBaseUrlKeyName)
                : CloudConfigurationManager.GetSetting(ControllerConstants.EmployerCommitmentsBaseUrlKeyName) + "/";
        }
    }
}