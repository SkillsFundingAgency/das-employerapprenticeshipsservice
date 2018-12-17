using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : Controller
    {
        [HttpGet]
        [Route]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("view")]
        public ActionResult ViewTeam(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/view?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("invite")]
        public ActionResult Invite(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/invite?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("invite/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/invite/next?{Request.QueryString}"));
        }


        [HttpGet]
        [Route("{invitationId}/cancel")]
        public ActionResult Cancel(string email, string invitationId, string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{invitationId}/cancel?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("{email}/remove/")]
        public ActionResult Remove(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/remove?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("{email}/role/change")]
        public ActionResult ChangeRole(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/role/change?{Request.QueryString}"));
        }


        [HttpGet]
        [Route("{email}/review/")]
        public ActionResult Review(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/review?{Request.QueryString}"));
        }

        [HttpGet]
        [Route("hideWizard")]
        public ActionResult HideWizard(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/hideWizard?{Request.QueryString}"));
        }
    }
}