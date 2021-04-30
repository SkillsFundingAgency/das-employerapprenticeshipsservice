using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [DasAuthorize("EmployerFeature.TransfersMatching")]
    [RoutePrefix("transfers")]
    public class TransfersController : Controller
    {
        [HttpGet]
        [Route("{hashedAccountId}")]
        public ActionResult Index(string hashedAccountId)
        {
            return View();
        }
    }
}