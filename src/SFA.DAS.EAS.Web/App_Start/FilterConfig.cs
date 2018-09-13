using System.Web.Mvc;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EAS.Web.Filters;
using SFA.DAS.EntityFramework;
using SFA.DAS.EntityFramework.Mvc;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ValidateFeatureFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new ViewModelFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()));
            filters.Add(new HandleErrorFilter());
        }
    }
}
