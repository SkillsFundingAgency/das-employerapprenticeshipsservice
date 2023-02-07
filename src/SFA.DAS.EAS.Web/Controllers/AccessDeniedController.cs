using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class AccessDeniedController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}