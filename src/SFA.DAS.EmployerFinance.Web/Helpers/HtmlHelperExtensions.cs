using System.Web.Mvc;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EmployerFinance.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static bool IsAuthorized(this HtmlHelper htmlHelper, string featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var isAuthorized = authorisationService.IsAuthorized(featureType);

            return isAuthorized;
        }
    }
}