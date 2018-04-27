using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ErrorController : Controller
    {

        public ActionResult Error()
        {
            return View();
        }


        public ActionResult NotFound()
        {
            return View("Error");
        }

        public ActionResult BadRequest()
        {
            return View("Error");
        }


    }
}