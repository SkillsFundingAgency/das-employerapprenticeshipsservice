using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class RestrictController : BaseController
    {
        public RestrictController(IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
        }

        // GET: Restrict
        public ActionResult Index()
        {
            return View();
        }

        [Route("refuse")]
        public ActionResult Refuse(string hashedAccountId)
        {
            ViewBag.AccountId = hashedAccountId;
          
            return View();
        }

    }
}