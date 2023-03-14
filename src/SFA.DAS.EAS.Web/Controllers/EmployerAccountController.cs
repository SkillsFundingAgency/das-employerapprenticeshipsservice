using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Domain.Configuration;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Route("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : Controller
    {
        public IConfiguration Configuration { get; set; }
        public EmployerAccountController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet]
        [Route("gatewayInform")]
        public ActionResult GatewayInform()
        {
            return Redirect(Url.EmployerAccountsAction("gatewayInform", Configuration));
        }

        [HttpGet]
        [Route("gateway")]
        public ActionResult Gateway()
        {
            return Redirect(Url.EmployerAccountsAction("gateway", Configuration));
        }

        [Route("gatewayResponse")]
        public ActionResult GateWayResponse()
        {
            return Redirect(Url.EmployerAccountsAction($"gatewayResponse{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("summary")]
        public ActionResult Summary()
        {
            return Redirect(Url.EmployerAccountsAction("summary", Configuration));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return Redirect(Url.EmployerAccountsAction("create", Configuration));
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public ActionResult RenameAccount(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("rename", Configuration));
        }
    }
}