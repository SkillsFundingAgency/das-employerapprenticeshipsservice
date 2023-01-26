using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class AccessDeniedController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("accessdenied", Name ="AccessDenied")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }
    }
}