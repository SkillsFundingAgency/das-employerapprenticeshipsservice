using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountPayeController : BaseController
    {
        public EmployerAccountPayeController(
            IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage) 
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
        }

        [HttpGet]
        [Route("schemes")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes"));
        }

        [HttpGet]
        [Route("schemes/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/next"));
        }

        [HttpGet]
        [Route("schemes/{empRef}/details")]
        public ActionResult Details(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/details"));
        }

        [HttpGet]
        [Route("schemes/gatewayInform")]
        public ActionResult GatewayInform(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/gatewayInform"));
        }
        
        [HttpGet]
        [Route("schemes/gateway")]
        public ActionResult GetGateway(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/gateway"));
        }

        [HttpGet]
        [Route("schemes/confirm")]
        public ActionResult ConfirmPayeScheme(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("/schemes/confirm"));
        }
        
        [HttpGet]
        [Route("schemes/{empRef}/remove")]
        public ActionResult Remove(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/remove"));
        }
    }
}