using System.Net;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("accessdenied")]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View();
        }

        [Route("error")]
        public ActionResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return View();
        }

        [Route("notfound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }
    }
}