using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts/{HashedAccountId}")]
    public class EmployerAccountPayeController : Controller
    {

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
            return Redirect(Url.EmployerAccountsAction($"/schemes/confirm{Request.QueryString}"));
        }
        
        [HttpGet]
        [Route("schemes/{empRef}/remove")]
        public ActionResult Remove(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/remove"));
        }
    }
}