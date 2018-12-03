using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("invitations")]
    public class InvitationController : Controller
    {
        [Route("invite")]
        public ActionResult Invite()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/invite", false));
        }

        [HttpGet]
        [AuthoriseActiveUser]
        [Route]
        public ActionResult All()
        {
            return Redirect(Url.EmployerAccountsAction("invitations", false));
        }

        [HttpGet]
        [Authorize]
        [Route("view")]
        public ActionResult Details(string invitationId)
        {
            return Redirect(Url.EmployerAccountsAction($"invitations/view?invitationId={invitationId}", false));
        }

        [HttpGet]
        [Route("register-and-accept")]
        public ActionResult AcceptInvitationNewUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/register-and-accept", false));
        }


        [HttpGet]
        [Route("accept")]
        public ActionResult AcceptInvitationExistingUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/accept", false));
        }
    }
}