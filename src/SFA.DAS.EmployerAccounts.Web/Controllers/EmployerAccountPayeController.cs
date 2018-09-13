using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountPayeController : Controller
    {
        [HttpGet]
        [Route("schemes")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("schemes"));
        }

        [HttpGet]
        [Route("schemes/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("schemes/next"));
        }

        [HttpGet]
        [Route("schemes/{empRef}/details")]
        public ActionResult Details(string hashedAccountId, string empRef)
        {
            return Redirect(Url.LegacyEasAccountAction($"schemes/{empRef}/details"));
        }

        [HttpGet]
        [Route("schemes/gatewayInform")]
        public ActionResult GatewayInform(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("schemes/gatewayInform"));
        }

        [HttpGet]
        [Route("schemes/gateway")]
        public ActionResult GetGateway(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("schemes/gateway"));
        }

        [HttpGet]
        [Route("schemes/confirm")]
        public ActionResult ConfirmPayeScheme(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("schemes/confirm"));
        }

        [HttpGet]
        [Route("schemes/{empRef}/remove")]
        public ActionResult Remove(string hashedAccountId, string empRef)
        {
            return Redirect(Url.LegacyEasAccountAction($"schemes/{empRef}/remove"));
        }
    }
}