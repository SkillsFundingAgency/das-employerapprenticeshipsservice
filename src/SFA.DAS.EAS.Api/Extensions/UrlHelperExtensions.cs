using System.Web.Mvc;
using SFA.DAS.EAS.Account.Api.Helpers;
using SFA.DAS.EAS.Domain.Configuration;
using UrlHelper = System.Web.Http.Routing.UrlHelper;

namespace SFA.DAS.EAS.Account.Api.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsApiAction(this UrlHelper helper, string pathAndQuery)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerApprenticeshipsServiceConfiguration>();
            var baseUrl = configuration.EmployerAccountsApiBaseUrl;
            baseUrl = "https://localhost:44330/";

            return Action(baseUrl, pathAndQuery);
        }


        public static string EmployerFinanceApiAction(this UrlHelper helper, string pathAndQuery)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerApprenticeshipsServiceConfiguration>();
            var baseUrl = configuration.EmployerFinanceApiBaseUrl;
            baseUrl = "https://localhost:44330/";

            return Action(baseUrl, pathAndQuery);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;
            var trimmedPath = path?.TrimStart('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{trimmedPath}".TrimEnd('/');
        }
    }
}