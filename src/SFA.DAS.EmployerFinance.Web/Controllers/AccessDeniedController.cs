using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class AccessDeniedController : Controller
    {
        [Route("accessdenied", Name ="AccessDenied")]
        public ActionResult Index()
        {
            return View();
        }
    }
}