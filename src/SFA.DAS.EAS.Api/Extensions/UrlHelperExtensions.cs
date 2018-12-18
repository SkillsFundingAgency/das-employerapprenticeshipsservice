using UrlHelper = System.Web.Http.Routing.UrlHelper;

namespace SFA.DAS.EAS.Account.Api.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Action(this UrlHelper helper, string baseUrl, string pathAndQuery)
        {
            return Action(baseUrl, pathAndQuery);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;
            var trimmedPath = path?.Trim('/') ?? string.Empty;

            return $"{trimmedBaseUrl}/{trimmedPath}";
        }
    }
}