using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    
    public class InvitationController : Controller
    {
        private readonly InvitationOrchestrator _invitationOrchestrator;
        private Claim _userIdClaim;

        public InvitationController(InvitationOrchestrator invitationOrchestrator)
        {
            if (invitationOrchestrator == null)
                throw new ArgumentNullException(nameof(invitationOrchestrator));
            _invitationOrchestrator = invitationOrchestrator;
            _userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
        }


        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> View(long invitationId)
        {
            if (_userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var invitation = await _invitationOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Accept(long invitationId)
        {
            if (_userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            await _invitationOrchestrator.AcceptInvitation(invitationId, _userIdClaim.Value);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(InviteTeamMemberViewModel model)
        {
            if (_userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            await _invitationOrchestrator.CreateInvitation(model, _userIdClaim.Value);

            return RedirectToAction("Index", "Home");
        }
    }
}