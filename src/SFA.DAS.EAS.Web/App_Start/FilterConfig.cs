using System.Web.Mvc;

using SFA.DAS.EAS.Web.Exceptions;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new InvalidStateExceptionFilter());
        }
    }
}
