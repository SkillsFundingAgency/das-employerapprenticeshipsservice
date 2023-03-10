using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : Controller
    {
        public EmployerApprenticeshipsServiceConfiguration Configuration { get; set; }
        public EmployerTeamController(EmployerApprenticeshipsServiceConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet]
        [Route("")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams?{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("view")]
        public ActionResult ViewTeam(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/view?{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("invite")]
        public ActionResult Invite(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/invite?{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("invite/next")]
        public ActionResult NextSteps(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/invite/next?{Request.QueryString}", Configuration));
        }


        [HttpGet]
        [Route("{invitationId}/cancel")]
        public ActionResult Cancel(string email, string invitationId, string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{invitationId}/cancel?{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("{email}/remove")]
        public ActionResult Remove(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/remove?{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("{email}/role/change")]
        public ActionResult ChangeRole(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/role/change?{Request.QueryString}", Configuration));
        }


        [HttpGet]
        [Route("{email}/review")]
        public ActionResult Review(string hashedAccountId, string email)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/{email}/review?{Request.QueryString}", Configuration));
        }

        [HttpGet]
        [Route("hideWizard")]
        public ActionResult HideWizard(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"teams/hideWizard?{Request.QueryString}", Configuration));
        }
    }
}