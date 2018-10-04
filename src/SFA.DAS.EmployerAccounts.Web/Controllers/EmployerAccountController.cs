using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : Controller
    {
        [HttpGet]
        [Route("gatewayInform")]
        public ActionResult GatewayInform()
        {
            return Redirect(Url.LegacyEasAccountAction("gatewayInform"));
        }

        [HttpGet]
        [Route("gateway")]
        public ActionResult Gateway()
        {
            return Redirect(Url.LegacyEasAccountAction("gateway"));
        }

        [Route("gatewayResponse")]
        public ActionResult GateWayResponse()
        {
            return Redirect(Url.LegacyEasAccountAction("gatewayResponse"));
        }

        [HttpGet]
        [Route("summary")]
        public ActionResult Summary()
        {
            return Redirect(Url.LegacyEasAccountAction("summary"));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return Redirect(Url.LegacyEasAccountAction("create"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public ActionResult RenameAccount(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("rename"));
        }
    }
}