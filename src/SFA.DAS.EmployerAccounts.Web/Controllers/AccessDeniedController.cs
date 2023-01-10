using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class AccessDeniedController : Controller
    {
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }
    }
}