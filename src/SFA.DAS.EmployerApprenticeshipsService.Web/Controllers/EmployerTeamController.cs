using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerTeamController : Controller
    {

        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

        public EmployerTeamController(IOwinWrapper owinWrapper, EmployerTeamOrchestrator employerTeamOrchestrator)
        {
            _owinWrapper = owinWrapper;
            _employerTeamOrchestrator = employerTeamOrchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int accountId)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var teamVieWModel = await _employerTeamOrchestrator.GetTeamMembers(accountId, userIdClaim.Value);
            return View(teamVieWModel);
        }

        [HttpGet]
        public ActionResult Invite(long accountId)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var model = new InviteTeamMemberViewModel
            {
                AccountId = accountId,
                Role = Role.Viewer
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            try
            {
                await _employerTeamOrchestrator.InviteTeamMember(model, userIdClaim.Value);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex.ErrorMessages);
                return View(model);
            }
            catch (Exception ex)
            {
                AddExceptionToModelError(ex);
                return View(model);
            }

            return RedirectToAction("Index", new { accountId = model.AccountId });
        }

        [HttpGet]
        public async Task<ActionResult> Review(long accountId, string email)
        {
            var model = await _employerTeamOrchestrator.Review(accountId, email);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Cancel(long id)
        {
            var invitation = await _employerTeamOrchestrator.GetInvitation(id);

            return View(invitation);
        }

        [HttpPost]
        public async Task<ActionResult> Cancel(long id, long accountId, int cancel)
        {
            if (cancel == 1)
            {
                var userIdClaim = ((ClaimsIdentity) System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
                if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

                await _employerTeamOrchestrator.Cancel(id, accountId, userIdClaim.Value);
            }
            return RedirectToAction("Index", new { accountId = accountId });
        }

        public async Task<ActionResult> Resend(long id, long accountId)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            await _employerTeamOrchestrator.Resend(id, accountId, userIdClaim.Value);

            return RedirectToAction("Index", new { accountId = accountId });
        }

        private void AddErrorsToModelState(Dictionary<string, string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private void AddExceptionToModelError(Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected exception: {ex.Message}");
        }
    }
}