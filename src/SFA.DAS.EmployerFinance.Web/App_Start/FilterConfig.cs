using System.Web.Mvc;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerFinance.Web.Filters;
using SFA.DAS.UnitOfWork.Mvc;

namespace SFA.DAS.EmployerFinance.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            filters.Add(new ValidateFeatureFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new ViewModelFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
        }
    }
}
