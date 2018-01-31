using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        [Route]
        public ActionResult Index(string hashedAccountId)
        {
            return View();
        }
        [Route("details")]
        public ActionResult Details(string hashedAccountId)
        {
            return View();
        }
    }
}