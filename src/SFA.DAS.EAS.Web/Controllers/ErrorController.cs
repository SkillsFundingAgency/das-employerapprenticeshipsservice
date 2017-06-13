using System.Net;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("Error")]
    public class ErrorController : Controller
    {
        private readonly ILog _logger;

        public ErrorController(ILog logger)
        {
            _logger = logger;
        }
        
        [Route]
        public ActionResult Index()
        {
            try
            {
                var lastError = Server.GetLastError();

                if (lastError != null)
                {
                    _logger.Error(lastError, $"Unhandled Exception: {lastError.Message}");
                }
            }
            catch
            {

            }
            return View("Error");
        }

        [Route("NotFound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        [Route("InvalidState/{hashedAccountId}")]
        public ActionResult InvalidState(string hashedAccountId)
        {
            var vm = new InvalidStateViewModel
            {
                HashedAccountId = hashedAccountId
            };

            return View(vm);
        }
    }
}