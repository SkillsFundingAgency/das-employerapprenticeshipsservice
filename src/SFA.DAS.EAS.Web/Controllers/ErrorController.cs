using System.Net;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class ErrorController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("accessdenied")]
        public Microsoft.AspNetCore.Mvc.ActionResult AccessDenied()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View();
        }

        [Route("error")]
        public Microsoft.AspNetCore.Mvc.ActionResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return View();
        }

        [Route("notfound")]
        public Microsoft.AspNetCore.Mvc.ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }
    }
}