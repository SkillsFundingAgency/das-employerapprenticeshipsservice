using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("gatewayInform")]
        public Microsoft.AspNetCore.Mvc.ActionResult GatewayInform()
        {
            return Redirect(Url.EmployerAccountsAction("gatewayInform"));
        }

        [HttpGet]
        [Route("gateway")]
        public Microsoft.AspNetCore.Mvc.ActionResult Gateway()
        {
            return Redirect(Url.EmployerAccountsAction("gateway"));
        }

        [Route("gatewayResponse")]
        public Microsoft.AspNetCore.Mvc.ActionResult GateWayResponse()
        {
            return Redirect(Url.EmployerAccountsAction($"gatewayResponse{Request.Url?.Query}"));
        }

        [HttpGet]
        [Route("summary")]
        public Microsoft.AspNetCore.Mvc.ActionResult Summary()
        {
            return Redirect(Url.EmployerAccountsAction("summary"));
        }

        [HttpGet]
        [Route("create")]
        public Microsoft.AspNetCore.Mvc.ActionResult Create()
        {
            return Redirect(Url.EmployerAccountsAction("create"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public Microsoft.AspNetCore.Mvc.ActionResult RenameAccount(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("rename"));
        }
    }
}