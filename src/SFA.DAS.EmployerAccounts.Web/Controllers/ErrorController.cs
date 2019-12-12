using System.Net;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{    
    public class ErrorController : BaseController
    {
        public ErrorController(
            IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
        }

        [Route("accessdenied")]
        public ActionResult AccessDenied(string hashedAccountId)
        
        {
            ViewBag.AccountId = hashedAccountId;

            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View();
        }
        
        [Route("error")]
        public ActionResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return View();
        }

        [Route("notfound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }
    }
}