using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    public class AccessDeniedController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}