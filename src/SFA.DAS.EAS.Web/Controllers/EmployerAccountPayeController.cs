using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountPayeController : Microsoft.AspNetCore.Mvc.Controller
    {

        [HttpGet]
        [Route("schemes")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes"));
        }

        [HttpGet]
        [Route("schemes/next")]
        public Microsoft.AspNetCore.Mvc.ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/next"));
        }

        [HttpGet]
        [Route("schemes/{empRef}/details")]
        public Microsoft.AspNetCore.Mvc.ActionResult Details(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/details"));
        }

        [HttpGet]
        [Route("schemes/gatewayInform")]
        public Microsoft.AspNetCore.Mvc.ActionResult GatewayInform(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/gatewayInform"));
        }
        
        [HttpGet]
        [Route("schemes/gateway")]
        public Microsoft.AspNetCore.Mvc.ActionResult GetGateway(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/gateway"));
        }

        [HttpGet]
        [Route("schemes/confirm")]
        public Microsoft.AspNetCore.Mvc.ActionResult ConfirmPayeScheme(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"/schemes/confirm{Request.Url?.Query}"));
        }
        
        [HttpGet]
        [Route("schemes/{empRef}/remove")]
        public Microsoft.AspNetCore.Mvc.ActionResult Remove(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/remove"));
        }
    }
}