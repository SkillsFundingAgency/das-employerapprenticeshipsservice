using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Filters;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new EnsureFeatureIsEnabledFilter(() => DependencyResolver.Current.GetService<ICurrentUserService>(), () => DependencyResolver.Current.GetService<IFeatureToggleService>()));
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new HandleErrorFilter());
            filters.Add(new MapViewModelToMessageFilter(() => DependencyResolver.Current.GetService<IMapper>()));
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()));
        }
    }
}
