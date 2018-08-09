using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : Controller
    {
        [HttpGet]
        [Route]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("teams"));
        }

        [HttpGet]
        [Route("view")]
        public ActionResult ViewTeam(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("teams/view"));
        }

        [HttpGet]
        [Route("invite")]
        public ActionResult Invite(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("teams/invite"));
        }

        [HttpGet]
        [Route("invite/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("teams/invite/next"));
        }

        [HttpGet]
        [Route("{invitationId}/cancel")]
        public ActionResult Cancel(string email, string invitationId, string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction($"teams/{invitationId}/cancel?email={email}"));
        }

        [HttpGet]
        [Route("{email}/remove/")]
        public ActionResult Remove(string hashedAccountId, string email)
        {
            return Redirect(Url.LegacyEasAccountAction($"teams/{email}/remove"));
        }

        [HttpGet]
        [Route("{email}/role/change")]
        public ActionResult ChangeRole(string hashedAccountId, string email)
        {
            return Redirect(Url.LegacyEasAccountAction($"teams/{email}/role/change"));
        }

        [HttpGet]
        [Route("{email}/review/")]
        public ActionResult Review(string hashedAccountId, string email)
        {
            return Redirect(Url.LegacyEasAccountAction($"teams/{email}/review"));
        }

        [HttpGet]
        [Route("hideWizard")]
        public ActionResult HideWizard(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("teams/hideWizard"));
        }
    }
}