using System.Web.Mvc;
using NLog;
using SFA.DAS.EAS.Web.Exceptions;

namespace SFA.DAS.EAS.Web.Filters
{
    public class HandleInvalidStateErrorFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(InvalidStateException))
            {
                LogManager.GetCurrentClassLogger().Error(filterContext.Exception, "Invalid state exception");

                // ToDo: Handle exception, Redirect or error page and User message?
                filterContext.ExceptionHandled = true;
            }
        }
    }
}