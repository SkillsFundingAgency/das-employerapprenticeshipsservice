using System.Web.Mvc;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountController : Controller
    {
        // GET: EmployerAccount
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(bool accepted)
        {
            return RedirectToAction("GovernmentGatewayConfirm");
        }

        [HttpGet]
        public ActionResult GovernmentGatewayConfirm()
        {
            return View();
        }
    }
}