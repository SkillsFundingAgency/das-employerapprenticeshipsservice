using System.Web.Mvc;

using SFA.DAS.EAS.Web.Exceptions;
using SFA.DAS.EAS.Web.Plumbing.Mvc;

namespace SFA.DAS.EAS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogAndHandleErrorAttribute());
            filters.Add(new InvalidStateExceptionFilter());
        }
    }
}
