using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Helpers;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsAction(this IUrlHelper helper, string path, IConfiguration config, bool includedAccountId = true)
        {
            var baseUrl = config["EmployerAccountsBaseUrl"] as string;

            return includedAccountId ? AccountAction(helper, baseUrl, path) : Action(baseUrl, path);
        }

        public static string EmployerCommitmentsAction(this IUrlHelper helper, string path, IConfiguration config)
        {
            var baseUrl = config.GetConnectionString("EmployerCommitmentsBaseUrl");

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerFinanceAction(this IUrlHelper helper, string path, IConfiguration config)
        {
            var baseUrl = config.GetConnectionString("EmployerFinanceBaseUrl");

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerProjectionsAction(this IUrlHelper helper, string path, IConfiguration config)
        {
            var baseUrl = config.GetConnectionString("EmployerProjectionsBaseUrl");

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerRecruitAction(this IUrlHelper helper, IConfiguration config)
        {
            var baseUrl = config.GetConnectionString("EmployerRecruitBaseUrl");

            return AccountAction(helper, baseUrl, "");
        }

        private static string AccountAction(IUrlHelper helper, string baseUrl, string path)
        {
            var hashedAccountId = helper.ActionContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName]; ;
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