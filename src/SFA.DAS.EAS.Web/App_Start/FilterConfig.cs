using System.Web.Mvc;
using SFA.DAS.EAS.Web.Filters;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GoogleAnalyticsFilter());
            filters.Add(new ViewModelFilter());
            filters.Add(new HandleErrorFilter());
            filters.Add(new HandleInvalidStateErrorFilter());
        }
    }
}
