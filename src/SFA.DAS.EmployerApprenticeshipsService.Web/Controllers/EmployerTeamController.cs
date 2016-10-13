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
        [Route("Teams")]
        public async Task<ActionResult> ViewTeam(string accountId)
        {
            var response = await _employerTeamOrchestrator.GetTeamMembers(accountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("Teams/Invite")]
        public ActionResult Invite(string accountId)
        {
            var model = new InviteTeamMemberViewModel
            {
                HashedId = accountId,
                Role = Role.Viewer
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/Invite")]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            try
            {
                var response = await _employerTeamOrchestrator.InviteTeamMember(model, OwinWrapper.GetClaimValue(@"sub"));

                return View("ViewTeam", response);
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
        }
        

        [HttpGet]
        [Route("Teams/{invitationId}/Cancel")]
        public async Task<ActionResult> Cancel(string email, string invitationId, string accountId)
        {
            var invitation = await _employerTeamOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{invitationId}/Cancel")]
        public async Task<ActionResult> Cancel(string invitationId, string email, string accountId, int cancel)
        {
            //TODO Flash message no longer works
            FlashMessageViewModel successMessage = null;
            if (cancel == 1)
            {
                await _employerTeamOrchestrator.Cancel(email, accountId, OwinWrapper.GetClaimValue(@"sub"));

                successMessage = new FlashMessageViewModel() {
                    Headline = "Invitation cancelled",
                    Message = $"You've cancelled the invitation sent to {email}",
                    Severity = FlashMessageSeverityLevel.Success
                };
            }

            return RedirectToAction("ViewTeam", new { accountId = accountId, flashMessage = successMessage });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/Resend")]
        public async Task<ActionResult> Resend(string accountId, string email)
        {
            await _employerTeamOrchestrator.Resend(email, accountId, OwinWrapper.GetClaimValue(@"sub"));

            TempData["successMessage"] = $"Invitation resent to {email}";

            return RedirectToAction("ViewTeam", new { accountId });
        }

        [HttpGet]
        [Route("Teams/{email}/Remove/")]
        public async Task<ActionResult> Remove(string accountId, string email)
        {
            var model = await _employerTeamOrchestrator.Review(accountId, email);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{email}/Remove")]
        public async Task<ActionResult> Remove(long userId, string accountId, string email, int remove)
        {
            
            try
            {
                FlashMessageViewModel successMessage = null;

                if (remove == 1)
                {
                    await _employerTeamOrchestrator.Remove(userId, accountId, OwinWrapper.GetClaimValue(@"sub"));

                    successMessage = new FlashMessageViewModel()
                    {
                        Headline = "Team member removed",
                        Message = $"You've removed the profile for {email}",
                        Severity = FlashMessageSeverityLevel.Success
                    };
                }

                return RedirectToAction("ViewTeam", new { accountId, flashMessage = successMessage });
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
        [Route("Teams/{email}/ChangeRole/")]
        public async Task<ActionResult> ChangeRole(string accountId, string email)
        {
            var teamMember = await _employerTeamOrchestrator.GetTeamMember(accountId, email);

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{email}/ChangeRole")]
        public async Task<ActionResult> ChangeRole(string accountId, string email, short role)
        {
            try
            {
                await _employerTeamOrchestrator.ChangeRole(accountId, email, role, OwinWrapper.GetClaimValue(@"sub"));

                var successMessage = new FlashMessageViewModel()
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Team member updated",
                    Message = $"{email} can now {RoleStrings.ToWhatTheyCanDoLower(role)}"
                };
                return RedirectToAction("ViewTeam", new { accountId = accountId, flashMessage = successMessage });
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
        [Route("Teams/{email}/Review/")]
        public async Task<ActionResult> Review(string accountId, string email)
        {
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