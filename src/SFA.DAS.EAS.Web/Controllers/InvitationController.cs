using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("invitations")]
    public class InvitationController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("invite")]
        public Microsoft.AspNetCore.Mvc.ActionResult Invite()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/invite", false));
        }

        [HttpGet]
        [AuthoriseActiveUser]
        [Route]
        public Microsoft.AspNetCore.Mvc.ActionResult All()
        {
            return Redirect(Url.EmployerAccountsAction("invitations", false));
        }

        [HttpGet]
        [DasAuthorize]
        [Route("view")]
        public Microsoft.AspNetCore.Mvc.ActionResult Details(string invitationId)
        {
            return Redirect(Url.EmployerAccountsAction($"invitations/view?invitationId={invitationId}", false));
        }

        [HttpGet]
        [Route("register-and-accept")]
        public Microsoft.AspNetCore.Mvc.ActionResult AcceptInvitationNewUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/register-and-accept", false));
        }


        [HttpGet]
        [Route("accept")]
        public Microsoft.AspNetCore.Mvc.ActionResult AcceptInvitationExistingUser()
        {
            return Redirect(Url.EmployerAccountsAction("invitations/accept", false));
        }
    }
}