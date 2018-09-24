using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerAccountsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerCommitmentsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerCommitmentsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerFinanceAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerFinanceBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerProjectionsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerProjectionsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerRecruitAction(this UrlHelper helper)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerRecruitBaseUrl;

            return AccountAction(helper, baseUrl, "");
        }

        public static string LegacyEasAccountAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string LegacyEasAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            return Action(baseUrl, path);
        }

        public static string LegacyEasActionWithoutHttpScheme(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            var url = Action(baseUrl, path);

            return RemoveHttpScheme(helper, url);
        }

        public static string RemoveHttpScheme(this UrlHelper helper, string url)
        {
            var formattedUrl = url;

            if (url.StartsWith("http://"))
            {
                formattedUrl = url.Substring("http://".Length);
            }
            else if (url.StartsWith("https://"))
            {
                formattedUrl = url.Substring("https://".Length);
            }

            return formattedUrl;
        }

        private static string AccountAction(UrlHelper helper, string baseUrl, string path)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";

            return Action(baseUrl, accountPath);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl.TrimEnd('/');

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}