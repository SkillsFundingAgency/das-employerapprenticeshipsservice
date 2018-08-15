using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("invitations")]
    public class InvitationController : Controller
    {
        [Route("invite")]
        public ActionResult Invite()
        {
            return Redirect(Url.LegacyEasAction("invitations/invite"));
        }

        [HttpGet]
        [AuthoriseActiveUser]
        [Route]
        public ActionResult All()
        {
            return Redirect(Url.LegacyEasAction("invitations"));
        }

        [HttpGet]
        [Authorize]
        [Route("view")]
        public ActionResult Details(string invitationId)
        {
            return Redirect(Url.LegacyEasAction($"invitations/view?invitationId={invitationId}"));
        }

        [HttpGet]
        [Route("register-and-accept")]
        public ActionResult AcceptInvitationNewUser()
        {
            return Redirect(Url.LegacyEasAction("invitations/register-and-accept"));
        }


        [HttpGet]
        [Route("accept")]
        public ActionResult AcceptInvitationExistingUser()
        {
            return Redirect(Url.LegacyEasAction("invitations/accept"));
        }
    }
}