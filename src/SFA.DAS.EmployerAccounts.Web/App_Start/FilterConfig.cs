using SFA.DAS.EmployerAccounts.Web.Filters;
using System.Web.Mvc;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ValidateFeatureFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new ViewModelFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
            filters.Add(new HandleErrorFilter());
        }
    }
}
