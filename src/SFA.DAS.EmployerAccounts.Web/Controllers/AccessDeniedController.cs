using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class AccessDeniedController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}