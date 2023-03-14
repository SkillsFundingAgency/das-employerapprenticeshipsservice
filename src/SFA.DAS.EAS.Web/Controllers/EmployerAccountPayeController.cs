using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts/{HashedAccountId}")]
    public class EmployerAccountPayeController : Controller
    {
        public IConfiguration Configuration { get; set; }
        public EmployerAccountPayeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet]
        [Route("schemes")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes", Configuration));
        }

        [HttpGet]
        [Route("schemes/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/next", Configuration));
        }

        [HttpGet]
        [Route("schemes/{empRef}/details")]
        public ActionResult Details(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/details", Configuration));
        }

        [HttpGet]
        [Route("schemes/gatewayInform")]
        public ActionResult GatewayInform(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/gatewayInform", Configuration));
        }
        
        [HttpGet]
        [Route("schemes/gateway")]
        public ActionResult GetGateway(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("schemes/gateway", Configuration));
        }

        [HttpGet]
        [Route("schemes/confirm")]
        public ActionResult ConfirmPayeScheme(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"/schemes/confirm{Request.QueryString}", Configuration));
        }
        
        [HttpGet]
        [Route("schemes/{empRef}/remove")]
        public ActionResult Remove(string hashedAccountId, string empRef)
        {
            return Redirect(Url.EmployerAccountsAction($"schemes/{empRef}/remove", Configuration));
        }
    }
}