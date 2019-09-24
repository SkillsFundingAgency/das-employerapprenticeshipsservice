using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.UnitOfWork.Mvc;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            filters.AddAuthorizationFilter();
            filters.AddUnauthorizedAccessExceptionFilter();
        }
    }
}
