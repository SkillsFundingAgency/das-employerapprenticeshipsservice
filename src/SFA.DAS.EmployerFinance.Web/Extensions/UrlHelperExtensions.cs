using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        private const string AccountsController = "accounts";

        public static string EmployerAccountsAction(this UrlHelper helper, string path, bool includedAccountId = true)
        {
            return EmployerAccountsAction(helper, AccountsController, path, includedAccountId);
        }

        public static string EmployerAccountsAction(this UrlHelper helper, string controller, string path, bool includedAccountId = true)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerAccountsBaseUrl;

            return includedAccountId ? Action(baseUrl, PathWithHashedAccountId(helper, controller, path)) : Action(baseUrl, $"{controller}/{path}");
        }

        public static string EmployerCommitmentsV2Action(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerCommitmentsV2BaseUrl;

            return NonAccountsAction(helper, baseUrl, path);
        }

        public static string LevyTransfersMatchingAccountAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.LevyTransferMatchingBaseUrl;

            return AccountsAction(helper, baseUrl, path);
        }

        public static string LevyTransfersMatchingAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.LevyTransferMatchingBaseUrl;

            return Action(baseUrl, path);
        }

        public static string ReservationsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.ReservationsBaseUrl;

            return AccountsAction(helper, baseUrl, path);
        }

        public static string EmployerFinanceAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerFinanceBaseUrl;

            return AccountsAction(helper, baseUrl, path);
        }

        public static string EmployerProjectionsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerProjectionsBaseUrl;

            return AccountsAction(helper, baseUrl, path);
        }

        public static string EmployerRecruitAction(this UrlHelper helper)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerRecruitBaseUrl;

            return AccountsAction(helper, baseUrl, "");
        }

        public static string LegacyEasAccountAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            return AccountsAction(helper, baseUrl, path);
        }

        public static string LegacyEasAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            return Action(baseUrl, path);
        }

        private static string AccountsAction(UrlHelper helper, string baseUrl, string path)
        {
            return Action(baseUrl, PathWithHashedAccountId(helper, AccountsController, path));
        }

        private static string NonAccountsAction(UrlHelper helper, string baseUrl, string path)
        {
            return Action(baseUrl, PathWithHashedAccountId(helper, string.Empty, path));
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl.TrimEnd('/');

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }

        private static string PathWithHashedAccountId(UrlHelper helper, string controller, string path)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];

            if(controller == string.Empty)
            {
                return hashedAccountId == null ? $"{path}" : $"{hashedAccountId}/{path}";
            }

            return hashedAccountId == null ? $"{controller}/{path}" : $"{controller}/{hashedAccountId}/{path}";
        }
    }
}