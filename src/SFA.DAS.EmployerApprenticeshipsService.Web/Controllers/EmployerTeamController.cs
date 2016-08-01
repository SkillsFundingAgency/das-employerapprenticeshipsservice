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
        public async Task<ActionResult> Index(int accountId, string successMessage)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var teamVieWModel = await _employerTeamOrchestrator.GetTeamMembers(accountId, userIdClaim);

            teamVieWModel.SuccessMessage = successMessage;

            return View(teamVieWModel);
        }

        [HttpGet]
        public ActionResult Invite(long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

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
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            if (userIdClaim == null) return RedirectToAction("Index", "Home");

            try
            {
                await _employerTeamOrchestrator.InviteTeamMember(model, userIdClaim);
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

            return RedirectToAction("Index", new { accountId = model.AccountId, successMessage = $"Invite sent to {model.Email}" });
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancel(string email, long accountId, int cancel)
        {
            if (cancel == 1)
            {
                var userIdClaim = ((ClaimsIdentity) System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
                if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

                await _employerTeamOrchestrator.Cancel(email, accountId, userIdClaim.Value);
            }
            return RedirectToAction("Index", new { accountId = accountId, successMessage = $"Cancelled invitation to {email}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Resend(long accountId, string email)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            await _employerTeamOrchestrator.Resend(email, accountId, userIdClaim.Value);

            return RedirectToAction("Index", new { accountId = accountId, successMessage = $"Invitation resent to {email}" });
        }

        [HttpGet]
        public async Task<ActionResult> Remove(long accountId, string email)
        {
            var model = await _employerTeamOrchestrator.Review(accountId, email);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Remove(long userId, long accountId, string email, int remove)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            try
            {
                if (remove == 1)
                    await _employerTeamOrchestrator.Remove(userId, accountId, userIdClaim.Value);

                return RedirectToAction("Index", new { accountId = accountId });
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex.ErrorMessages);
            }
            catch (Exception ex)
            {
                AddExceptionToModelError(ex);
            }

            var model = await _employerTeamOrchestrator.Review(accountId, email);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeRole(long accountId, string email)
        {
            var teamMember = await _employerTeamOrchestrator.GetTeamMember(accountId, email);

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeRole(long accountId, string email, int role)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            try
            {
                await _employerTeamOrchestrator.ChangeRole(accountId, email, role, userIdClaim.Value);

                return RedirectToAction("Index");
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex.ErrorMessages);
            }
            catch (Exception ex)
            {
                AddExceptionToModelError(ex);
            }

            var teamMember = await _employerTeamOrchestrator.GetTeamMember(accountId, email);
            return View(teamMember);
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