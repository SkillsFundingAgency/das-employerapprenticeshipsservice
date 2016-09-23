using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{accountId}")]
    public class EmployerTeamController : BaseController
    {
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

        public EmployerTeamController(IOwinWrapper owinWrapper, EmployerTeamOrchestrator employerTeamOrchestrator, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            _employerTeamOrchestrator = employerTeamOrchestrator;
        }

        [HttpGet]
        [Route]
        public async Task<ActionResult> Index(string accountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");

            var response = await _employerTeamOrchestrator.GetAccount(accountId, userIdClaim);

            return View(response);
      
        }

        [HttpGet]
        [Route("View")]
        public async Task<ActionResult> View(int accountId)
        {
            FlashMessageViewModel flashMessage = TempData["flashMessage"] as FlashMessageViewModel;

            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Index", "Home");

            var response = await _employerTeamOrchestrator.GetTeamMembers(accountId, userIdClaim);

            // DANGER: Directly injected messages trump orchestrator responses
            if (flashMessage != null) response.FlashMessage = flashMessage;
            return View(response);
        }

        [HttpGet]
        [Route("Invite")]
        public ActionResult Invite(long accountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
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
        [Route("Invite")]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
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

            var successMessage = new FlashMessageViewModel()
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = "Invitation sent",
                Message = $"You've sent an invitation to {model.Email}"
            };

            TempData["flashMessage"] = successMessage;
            return RedirectToAction("View", new { accountId = model.AccountId });
        }
        

        [HttpGet]
        [Route("Cancel")]
        public async Task<ActionResult> Cancel(long id)
        {
            var invitation = await _employerTeamOrchestrator.GetInvitation(id);

            return View(invitation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Cancel")]
        public async Task<ActionResult> Cancel(string email, long accountId, int cancel)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            FlashMessageViewModel successMessage = null;
            if (cancel == 1)
            {
                await _employerTeamOrchestrator.Cancel(email, accountId, userIdClaim.Value);

                successMessage = new FlashMessageViewModel() {
                    Headline = "Invitation cancelled",
                    Message = $"You've cancelled the invitation sent to {email}",
                    Severity = FlashMessageSeverityLevel.Success
                };
            }

            return RedirectToAction("View", new { accountId = accountId, flashMessage = successMessage });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Resend")]
        public async Task<ActionResult> Resend(long accountId, string email)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            await _employerTeamOrchestrator.Resend(email, accountId, userIdClaim.Value);

            TempData["successMessage"] = $"Invitation resent to {email}";

            return RedirectToAction("View", new { accountId = accountId });
        }

        [HttpGet]
        [Route("Remove")]
        public async Task<ActionResult> Remove(long accountId, string email)
        {
            var model = await _employerTeamOrchestrator.Review(accountId, email);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Remove")]
        public async Task<ActionResult> Remove(long userId, long accountId, string email, int remove)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            try
            {
                FlashMessageViewModel successMessage = null;

                if (remove == 1)
                {
                    await _employerTeamOrchestrator.Remove(userId, accountId, userIdClaim.Value);

                    successMessage = new FlashMessageViewModel()
                    {
                        Headline = "Team member removed",
                        Message = $"You've removed the profile for {email}",
                        Severity = FlashMessageSeverityLevel.Success
                    };
                }

                return RedirectToAction("View", new { accountId = accountId, flashMessage = successMessage });
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
        [Route("ChangeRole")]
        public async Task<ActionResult> ChangeRole(long accountId, string email)
        {
            var teamMember = await _employerTeamOrchestrator.GetTeamMember(accountId, email);

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ChangeRole")]
        public async Task<ActionResult> ChangeRole(long accountId, string email, short role)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            try
            {
                await _employerTeamOrchestrator.ChangeRole(accountId, email, role, userIdClaim.Value);

                var successMessage = new FlashMessageViewModel()
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Team member updated",
                    Message = $"{email} can now {RoleStrings.ToWhatTheyCanDoLower(role)}"
                };
                return RedirectToAction("View", new { accountId = accountId, flashMessage = successMessage });
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


        [HttpGet]
        [Route("Review")]
        public async Task<ActionResult> Review(int accountId, string email)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var invitation = await _employerTeamOrchestrator.GetTeamMember(accountId, email);

            return View(invitation);
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