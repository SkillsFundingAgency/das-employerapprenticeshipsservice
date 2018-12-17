﻿using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts")]
    [AuthoriseActiveUser]
    public class EmployerAccountController : Controller
    {
        [HttpGet]
        [Route("gatewayInform")]
        public ActionResult GatewayInform()
        {
            return Redirect(Url.EmployerAccountsAction("gatewayInform"));
        }

        [HttpGet]
        [Route("gateway")]
        public ActionResult Gateway()
        {
            return Redirect(Url.EmployerAccountsAction("gateway"));
        }

        [Route("gatewayResponse")]
        public ActionResult GateWayResponse()
        {
            return Redirect(Url.EmployerAccountsAction("gatewayResponse"));
        }

        [HttpGet]
        [Route("summary")]
        public ActionResult Summary()
        {
            return Redirect(Url.EmployerAccountsAction("summary"));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return Redirect(Url.EmployerAccountsAction("create"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/rename")]
        public ActionResult RenameAccount(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction("rename"));
        }
    }
}