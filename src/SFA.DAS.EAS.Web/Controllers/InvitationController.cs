using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("invitations")]
    public class InvitationController : Controller
    {
        [Route("invite")]
        public ActionResult Invite()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/invite"));
        }

        [HttpGet]
        [AuthoriseActiveUser]
        [Route]
        public ActionResult All()
        {
            return Redirect(Url.EmployerAccountsAction("invitations"));
        }

        [HttpGet]
        [Authorize]
        [Route("view")]
        public ActionResult Details(string invitationId)
        {
            return Redirect(Url.EmployerAccountsAction($"invitations/view?invitationId={invitationId}"));
        }

        [HttpGet]
        [Route("register-and-accept")]
        public ActionResult AcceptInvitationNewUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/register-and-accept"));
        }


        [HttpGet]
        [Route("accept")]
        public ActionResult AcceptInvitationExistingUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/accept"));
        }
    }
}