using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EAS.Web.Helpers;

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

        public static string EmployerFinanceAction(this UrlHelper helper, string path, dynamic routeValues)
        {

            if (routeValues == null) return Action(helper, path, ControllerConstants.EmployerFinanceWebBaseUrlKeyName);
            
            var keyValueList = ((object)routeValues).ToDictionary();

            if (!keyValueList.Any()) return Action(helper, path, ControllerConstants.EmployerFinanceWebBaseUrlKeyName);

            var pathQuery = keyValueList.Aggregate(string.Empty, (current, item) => current + EscapeRouteParameter(item))
                                        .TrimStart('&');
            return Action(helper, $"{path.TrimEnd('/')}?{pathQuery}",ControllerConstants.EmployerFinanceWebBaseUrlKeyName);
        }

        private static string EscapeRouteParameter(KeyValuePair<string, object> item)
        {
            return $"&{item.Key.HtmlEncode()}={FormatParameterValue(item).HtmlEncode()}";
        }

        private static string FormatParameterValue(KeyValuePair<string, object> item)
        {
            var itemType = item.Value.GetType();
            if (itemType == typeof(DateTime) || itemType == typeof(DateTimeOffset)
            )
            {
                return $"{item.Value:O}";
            }
            return $"{item.Value}";
        }

        private static string Action(UrlHelper helper, string path, string baseUrlKeyName)
        {
            var baseUrl = CloudConfigurationManager.GetSetting(baseUrlKeyName)?.TrimEnd('/');
            var hashedAccountId =
                helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            return $"{baseUrl}/accounts/{hashedAccountId}/{path}";
        }
    }
}