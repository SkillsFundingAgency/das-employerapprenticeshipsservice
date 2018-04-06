using System.Web.Mvc;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Web.Filters;

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
