using System.Web.Mvc;

using NLog;

namespace SFA.DAS.EAS.Web.Exceptions
{
    public class InvalidStateExceptionFilter : FilterAttribute, IExceptionFilter
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