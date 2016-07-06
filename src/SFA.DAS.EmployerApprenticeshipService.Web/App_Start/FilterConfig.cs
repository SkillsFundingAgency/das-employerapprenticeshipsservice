using System.Web.Mvc;

namespace SFA.DAS.EmployerApprenticeshipService.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
