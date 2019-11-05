using SFA.DAS.EmployerAccounts.Web.Filters;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.UnitOfWork.Mvc.Extensions;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            filters.Add(new GoogleAnalyticsFilter());
            filters.AddAuthorizationFilter();
            filters.AddUnauthorizedAccessExceptionFilter();
        }
    }
}
