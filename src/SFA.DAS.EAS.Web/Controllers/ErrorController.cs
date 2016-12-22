using System.Net;
using System.Web.Mvc;
using NLog;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger _logger;

        public ErrorController(ILogger logger)
        {
            _logger = logger;
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        public ActionResult Index()
        {
            try
            {
                var lastError = Server.GetLastError();

                if (lastError != null)
                {
                    _logger.Error(lastError);
                }
            }
            catch 
            {
                
            }
            return View("Error");
        }
    }
}