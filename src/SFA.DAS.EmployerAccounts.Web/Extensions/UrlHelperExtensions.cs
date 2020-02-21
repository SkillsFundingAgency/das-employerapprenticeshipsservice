using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
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

        public static string EmployerCommitmentsAction(this UrlHelper helper, string path)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerCommitmentsBaseUrl;
            
            return AccountAction(helper, baseUrl, path);
        }

        public static string EmployerCommitmentsV2Action(this UrlHelper helper, string routeName)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerCommitmentsV2BaseUrl;

            return CommitmentAction(helper, baseUrl, routeName);
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

        public static string FavouritesAction(this UrlHelper helper, string path = "")
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            var baseUrl = configuration.EmployerFavouritesBaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        private static string AccountAction(UrlHelper helper, string baseUrl, string path)
        {
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var accountPath = hashedAccountId == null ? $"accounts/{path}" : $"accounts/{hashedAccountId}/{path}";

            return Action(baseUrl, accountPath);
        }

        private static string CommitmentAction(UrlHelper helper, string baseUrl, string routeName)
        {            
            var hashedAccountId = helper.RequestContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            var model = helper.RequestContext.RouteData.Values["model"]  as AccountDashboardViewModel;
            var hashedCohortReference = model?.CallToActionViewModel?.HashedCohortReference ?? string.Empty;
            var hashedDraftApprenticeshipId = model?.CallToActionViewModel?.HashedDraftApprenticeshipId ?? string.Empty;
            string commitmentPath;
            switch (routeName)
            {
                case ControllerConstants.ApproveOrRejectApprentice:
                case ControllerConstants.ViewApprenticeBeforeApprove:
                    commitmentPath = $"{hashedAccountId}/unapproved/{hashedCohortReference}";
                    break;
                default:
                    commitmentPath = $"{hashedAccountId}/unapproved/{hashedCohortReference}/apprentices/{hashedDraftApprenticeshipId}";
                    break;
            }
            
            return Action(baseUrl, commitmentPath);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}