using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    
    public class InvitationController : Controller
    {
        private readonly InvitationOrchestrator _invitationOrchestrator;
        private readonly string _userIdClaim;

        public InvitationController(InvitationOrchestrator invitationOrchestrator, IOwinWrapper owinWrapper)
        {
            if (invitationOrchestrator == null)
                throw new ArgumentNullException(nameof(invitationOrchestrator));
            _invitationOrchestrator = invitationOrchestrator;
            _userIdClaim = owinWrapper.GetClaimValue("sub");
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
        public async Task<ActionResult> View(long invitationId)
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
        public async Task<ActionResult> Accept(long invitationId)
        {
            if (string.IsNullOrEmpty(_userIdClaim))
            {
                return RedirectToAction("Index", "Home");
            }

            await _invitationOrchestrator.AcceptInvitation(invitationId, _userIdClaim);

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