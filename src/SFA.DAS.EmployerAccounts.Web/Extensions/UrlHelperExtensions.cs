using Microsoft.Azure;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
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

        private static string Action(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];

            return $"{baseUrl}/accounts/{hashedAccountId}/{path}";
        }
    }
}
