using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route]
        public Microsoft.AspNetCore.Mvc.ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("view")]
        public Microsoft.AspNetCore.Mvc.ActionResult ViewTeam(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/view?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("invite")]
        public Microsoft.AspNetCore.Mvc.ActionResult Invite(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/invite?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("invite/next")]
        public Microsoft.AspNetCore.Mvc.ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/invite/next?{Request.QueryString}"));
        }


        [HttpGet]
        [Route("{invitationId}/cancel")]
        public Microsoft.AspNetCore.Mvc.ActionResult Cancel(string email, string invitationId, string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{invitationId}/cancel?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("{email}/remove")]
        public Microsoft.AspNetCore.Mvc.ActionResult Remove(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/remove?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("{email}/role/change")]
        public Microsoft.AspNetCore.Mvc.ActionResult ChangeRole(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/role/change?{Request.QueryString}"));
        }


        [HttpGet]
        [Route("{email}/review")]
        public Microsoft.AspNetCore.Mvc.ActionResult Review(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/review?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("hideWizard")]
        public Microsoft.AspNetCore.Mvc.ActionResult HideWizard(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/hideWizard?{Request.QueryString}"));
        }
    }
}