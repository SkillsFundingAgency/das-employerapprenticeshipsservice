using System.Web.Mvc;
using SFA.DAS.EAS.Support.Web.Helpers;
using SFA.DAS.EAS.Support.Web.Configuration;

namespace SFA.DAS.EAS.Support.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string StaffLoginAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<IWebConfiguration>();
            var baseUrl = configuration.EmployerAccountsConfiguration.EmployerAccountsBaseUrl;

            return StaffAction(helper, baseUrl, path);
        }

        private static string StaffAction(UrlHelper helper, string baseUrl, string path)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"{path}" : $"{hashedAccountId}/{path}";

            return Action(baseUrl, accountPath);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}