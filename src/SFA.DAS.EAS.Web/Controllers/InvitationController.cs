using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    
    [RoutePrefix("invitations")]
    public class InvitationController : BaseController
    {
        private readonly InvitationOrchestrator _invitationOrchestrator;
        
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public InvitationController(InvitationOrchestrator invitationOrchestrator, IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList, EmployerApprenticeshipsServiceConfiguration configuration) 
            : base(owinWrapper, featureToggle)
        {
            if (invitationOrchestrator == null)
                throw new ArgumentNullException(nameof(invitationOrchestrator));
            _invitationOrchestrator = invitationOrchestrator;
            _configuration = configuration;
            
        }

        [Route("invite")]
        public ActionResult Invite()
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue("sub")))
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        [AuthoriseActiveUser]
        [Route]
        public async Task<ActionResult> All()
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue("sub")))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = await _invitationOrchestrator.GetAllInvitationsForUser(OwinWrapper.GetClaimValue("sub"));

            return View(model);
        }

        [HttpGet]
        [Authorize]
        [Route("view")]
        public async Task<ActionResult> View(string invitationId)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue("sub")))
            {
                return RedirectToAction("Index", "Home");
            }

            var invitation = await _invitationOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("accept")]
        public async Task<ActionResult> Accept(long invitation, UserInvitationsViewModel model)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue("sub")))
            {
                return RedirectToAction("Index", "Home");
            }

            var invitationItem = model.Invitations.SingleOrDefault(c => c.Id == invitation);

            if (invitationItem == null)
            {
                return RedirectToAction("Index", "Home");
            }

            await _invitationOrchestrator.AcceptInvitation(invitationItem.Id, OwinWrapper.GetClaimValue("sub"));
            
            TempData["successHeader"] = "Invitation accepted";
            TempData["successMessage"] = $"You can now access the {invitationItem.AccountName} account";

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<ActionResult> Create(InviteTeamMemberViewModel model)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue("sub")))
            {
                return RedirectToAction("Index", "Home");     
            }

            await _invitationOrchestrator.CreateInvitation(model, OwinWrapper.GetClaimValue("sub"));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("register-and-accept")]
        public ActionResult AcceptInvitationNewUser()
        {
            var schema = System.Web.HttpContext.Current.Request.Url.Scheme;
            var authority = System.Web.HttpContext.Current.Request.Url.Authority;
            var c = new Constants(_configuration.Identity);
            return new RedirectResult($"{c.RegisterLink()}{schema}://{authority}/invitations");
        }


        [HttpGet]
        [Route("accept")]
        public ActionResult AcceptInvitationExistingUser()
        {
            return RedirectToAction("All");
        }


    }
}