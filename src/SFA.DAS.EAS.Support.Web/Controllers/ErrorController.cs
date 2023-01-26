using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Support.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ErrorController : Microsoft.AspNetCore.Mvc.Controller
    {

        public Microsoft.AspNetCore.Mvc.ActionResult Error()
        {
            return View();
        }


        public Microsoft.AspNetCore.Mvc.ActionResult NotFound()
        {
            return View("Error");
        }

        public Microsoft.AspNetCore.Mvc.ActionResult BadRequest()
        {
            return View("Error");
        }


    }
}