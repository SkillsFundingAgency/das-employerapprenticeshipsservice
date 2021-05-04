using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [DasAuthorize("EmployerFeature.TransfersMatching")]
    [RoutePrefix("accounts/{HashedAccountId}")] public class TransfersController : Controller
    {
        [HttpGet]
        [Route("transfers")]
        public ActionResult Index(string hashedAccountId)
        {
            return View();
        }
    }
}