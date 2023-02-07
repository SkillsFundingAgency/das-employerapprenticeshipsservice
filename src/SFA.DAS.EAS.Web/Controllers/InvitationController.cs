using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Route("invitations")]
    public class InvitationController : Controller
    {
        [Route("invite")]
        public ActionResult Invite()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/invite", false));
        }

        [HttpGet("")]
        [AuthoriseActiveUser]
        public ActionResult All()
        {
            return Redirect(Url.EmployerAccountsAction("invitations", false));
        }

        [HttpGet]
        [DasAuthorize]
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