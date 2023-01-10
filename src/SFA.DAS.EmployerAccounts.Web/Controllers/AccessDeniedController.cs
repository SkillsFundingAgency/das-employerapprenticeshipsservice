using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class AccessDeniedController : Microsoft.AspNetCore.Mvc.Controller
    {
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }
    }
}