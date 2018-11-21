using System.Web.Http;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;
using WebApi.StructureMap;
using UrlHelper = System.Web.Http.Routing.UrlHelper;

namespace SFA.DAS.EAS.Account.Api.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmployerAccountsApiAction(this UrlHelper helper, string pathAndQuery)
        {
            var container = GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>();
            var configuration = container.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var baseUrl = configuration.EmployerAccountsApiBaseUrl;

            return Action(baseUrl, pathAndQuery);
        }


        public static string EmployerFinanceApiAction(this UrlHelper helper, string pathAndQuery)
        {
            var container = GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>();
            var configuration = container.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var baseUrl = configuration.EmployerFinanceApiBaseUrl;

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