using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return View();
        }
    }
}