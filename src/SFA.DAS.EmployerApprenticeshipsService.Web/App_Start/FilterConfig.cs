using System.Web.Mvc;

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
