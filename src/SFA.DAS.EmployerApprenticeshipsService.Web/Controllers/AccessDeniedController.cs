using System.Web.Mvc;

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