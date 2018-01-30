using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Filters;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var dependencyResolver = DependencyResolver.Current;

            filters.Add(new EnsureFeatureIsEnabledFilter(() => dependencyResolver.GetService<ICurrentUserService>(), () => dependencyResolver.GetService<IFeatureToggleService>()));
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new HandleErrorFilter());
            filters.Add(new MapViewModelToMessageFilter(() => dependencyResolver.GetService<IMapper>()));
        }
    }
}
