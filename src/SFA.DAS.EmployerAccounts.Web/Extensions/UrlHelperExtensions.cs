using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerAccountsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerCommitmentsV2Action(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerCommitmentsV2BaseUrl;

            return NonAccountsAction(helper, baseUrl, path);
        }
        public static string LevyTransfersMatchingAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.LevyTransferMatchingBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string ReservationsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.ReservationsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerFinanceAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerFinanceBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerIncentivesAction(this UrlHelper helper, string path = "")
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerIncentivesBaseUrl;
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            return Action(baseUrl, $"{hashedAccountId}/{path}");
        }

        public static string EmployerProjectionsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerProjectionsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerRecruitAction(this UrlHelper helper, string path = "")
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerRecruitBaseUrl;
            
            return AccountAction(helper, baseUrl, path);
        }

        public static string ProviderRelationshipsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.ProviderRelationshipsBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string LegacyEasAccountAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string LegacyEasAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerPortalBaseUrl;

            return Action(baseUrl, path);
        }

        private static string AccountAction(UrlHelper helper, string baseUrl, string path)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";

            return Action(baseUrl, accountPath);
        }

        // unlike the rest of the services within MA - commitments v2 does not have 'accounts/' in its urls
        // Nor does Employer Feedback
        private static string NonAccountsAction(UrlHelper helper, string baseUrl, string path)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var commitmentPath = hashedAccountId == null ? $"{path}" : $"{hashedAccountId}/{path}";
           
            return Action(baseUrl, commitmentPath);
        }

        public static string EmployerFeedbackAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerFeedbackBaseUrl;

            return NonAccountsAction(helper, baseUrl, path);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}