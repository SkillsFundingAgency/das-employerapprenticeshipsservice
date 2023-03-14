using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Route("invitations")]
    public class InvitationController : Controller
    {
        public IConfiguration Configuration { get; set; }
        public InvitationController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [Route("invite")]
        public ActionResult Invite()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/invite", Configuration, false));
        }

        [HttpGet("")]
        [AuthoriseActiveUser]
        public ActionResult All()
        {
            return Redirect(Url.EmployerAccountsAction("invitations", Configuration, false));
        }

        [HttpGet]
        [DasAuthorize]
        [Route("view")]
        public ActionResult Details(string invitationId)
        {
            return Redirect(Url.EmployerAccountsAction($"invitations/view?invitationId={invitationId}", Configuration, false));
        }

        [HttpGet]
        [Route("register-and-accept")]
        public ActionResult AcceptInvitationNewUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/register-and-accept", Configuration, false));
        }


        [HttpGet]
        [Route("accept")]
        public ActionResult AcceptInvitationExistingUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/accept", Configuration, false));
        }
    }
}