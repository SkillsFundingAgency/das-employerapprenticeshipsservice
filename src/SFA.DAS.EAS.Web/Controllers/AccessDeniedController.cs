using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class AccessDeniedController : Microsoft.AspNetCore.Mvc.Controller
    {
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }
    }
}