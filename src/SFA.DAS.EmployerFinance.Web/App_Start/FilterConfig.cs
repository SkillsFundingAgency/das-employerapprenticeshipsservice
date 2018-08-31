using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Web.Filters;

namespace SFA.DAS.EmployerFinance.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()));
            filters.Add(new HandleErrorFilter());
        }
    }
}
