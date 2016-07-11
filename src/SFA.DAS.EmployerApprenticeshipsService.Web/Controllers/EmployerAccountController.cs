using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}