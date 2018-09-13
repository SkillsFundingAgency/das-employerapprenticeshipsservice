using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EntityFramework;
using SFA.DAS.UnitOfWork.Mvc;

namespace SFA.DAS.EmployerFinance.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            filters.Add(new EntityFramework.Mvc.UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()));
            filters.Add(new HandleErrorFilter());
        }
    }
}
