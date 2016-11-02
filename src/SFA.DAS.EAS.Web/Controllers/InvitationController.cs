using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.ViewModels;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    
    public class InvitationController : BaseController
    {
        private readonly InvitationOrchestrator _invitationOrchestrator;
        private readonly string _userIdClaim;

        public InvitationController(InvitationOrchestrator invitationOrchestrator, IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (invitationOrchestrator == null)
                throw new ArgumentNullException(nameof(invitationOrchestrator));
            _invitationOrchestrator = invitationOrchestrator;
            _userIdClaim = OwinWrapper.GetClaimValue("sub");
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult> All()
        {
            if (string.IsNullOrEmpty(_userIdClaim))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = await _invitationOrchestrator.GetAllInvitationsForUser(_userIdClaim);

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> View(string invitationId)
        {
            if (string.IsNullOrEmpty(_userIdClaim))
            {
                return RedirectToAction("Index", "Home");
            }

            var invitation = await _invitationOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Accept(long invitation, UserInvitationsViewModel model)
        {
            if (string.IsNullOrEmpty(_userIdClaim))
            {
                return RedirectToAction("Index", "Home");
            }

            var invitationItem = model.Invitations.SingleOrDefault(c => c.Id == invitation);

            if (invitationItem == null)
            {
                return RedirectToAction("Index", "Home");
            }

            await _invitationOrchestrator.AcceptInvitation(invitationItem.Id, _userIdClaim);
            //TODO incorrect message when accepting an invite
            TempData["successHeader"] = "Invitation Accepted";
            TempData["successCompany"] = invitationItem.AccountName;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InviteTeamMemberViewModel model)
        {
            if (string.IsNullOrEmpty(_userIdClaim))
            {
                return RedirectToAction("Index", "Home");     
            }

            await _invitationOrchestrator.CreateInvitation(model, _userIdClaim);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(_userIdClaim))
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}