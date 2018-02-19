using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerCommitmentsAction(this UrlHelper helper, string controllerName, string actionName = "")
        {
            var baseUrl = CloudConfigurationManager.GetSetting(ControllerConstants.EmployerCommitmentsBaseUrlKeyName).EndsWith("/")
                ? CloudConfigurationManager.GetSetting(ControllerConstants.EmployerCommitmentsBaseUrlKeyName)
                : CloudConfigurationManager.GetSetting(ControllerConstants.EmployerCommitmentsBaseUrlKeyName) + "/";

            var accountId = helper.RequestContext.RouteData.Values[ControllerConstants.HashedAccountIdKeyName];

            return $"{baseUrl}accounts/{accountId}/{controllerName}/{actionName}";
        }

        public static string EmployerRecruitAction(this UrlHelper helper, string controllerName, string actionName = "")
        {
            var baseUrl = CloudConfigurationManager.GetSetting(ControllerConstants.EmployerRecruitControllerName).EndsWith("/")
                ? CloudConfigurationManager.GetSetting(ControllerConstants.EmployerRecruitControllerName)
                : CloudConfigurationManager.GetSetting(ControllerConstants.EmployerRecruitControllerName) + "/";

            var accountId = helper.RequestContext.RouteData.Values[ControllerConstants.HashedAccountIdKeyName];

            return $"{baseUrl}accounts/{accountId}/{controllerName}/{actionName}";
        }
    }
}