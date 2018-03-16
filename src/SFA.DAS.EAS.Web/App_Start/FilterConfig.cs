using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Filters;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ValidateFeatureFilter(() => DependencyResolver.Current.GetService<IOperationAuthorisationService>(), () => DependencyResolver.Current.GetService<IMembershipService>()));
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new MapViewModelToMessageFilter(() => DependencyResolver.Current.GetService<IMapper>()));
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()));
            filters.Add(new HandleErrorFilter());
        }
    }
}
