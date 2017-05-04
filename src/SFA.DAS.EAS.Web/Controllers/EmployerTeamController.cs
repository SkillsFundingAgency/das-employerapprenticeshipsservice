using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : BaseController
    {
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

        public EmployerTeamController(IOwinWrapper owinWrapper, EmployerTeamOrchestrator employerTeamOrchestrator, 
            IFeatureToggle featureToggle, IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage) 
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _employerTeamOrchestrator = employerTeamOrchestrator;
        }

        [HttpGet]
        [Route]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");

            var response = await _employerTeamOrchestrator.GetAccount(hashedAccountId, userIdClaim);

            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage!=null)
            {
                response.FlashMessage = flashMessage;
                response.Data.EmployerAccountType = flashMessage.HiddenFlashMessageInformation;
            }

            return View(response);
        }

        [HttpGet]
        [Route("view")]
        public async Task<ActionResult> ViewTeam(string hashedAccountId)
        {
            
            var response = await _employerTeamOrchestrator.GetTeamMembers(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                response.FlashMessage = flashMessage;
            }

            return View(response);
        }

        [HttpGet]
        [Route("invite")]
        public async Task<ActionResult> Invite(string hashedAccountId)
        {
            var response = await _employerTeamOrchestrator.GetNewInvitation(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("invite")]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var response = await _employerTeamOrchestrator.InviteTeamMember(model, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                var flashMessage = new FlashMessageViewModel
                {
                    HiddenFlashMessageInformation = "page-invite-team-member-sent",
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Invitation sent",
                    Message = $"You've sent an invitation to <strong>{model.Email}</strong>"
                };
                AddFlashMessageToCookie(flashMessage);

                return RedirectToAction("ViewTeam");
            }
                
           
            model.ErrorDictionary = response.FlashMessage.ErrorMessages; 
            var errorResponse = new OrchestratorResponse<InviteTeamMemberViewModel>
            {
                Data = model,
                FlashMessage = response.FlashMessage,
            };

            return View(errorResponse);
        }
        
        [HttpGet]
        [Route("{invitationId}/cancel")]
        public async Task<ActionResult> Cancel(string email, string invitationId, string hashedAccountId)
        {
            var invitation = await _employerTeamOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{invitationId}/cancel")]
        public async Task<ActionResult> Cancel(string invitationId, string email, string hashedAccountId, int cancel)
        {
            if (cancel != 1)
                return RedirectToAction("ViewTeam", new { HashedAccountId = hashedAccountId });

            var response =  await _employerTeamOrchestrator.Cancel(email, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View("ViewTeam", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("resend")]
        public async Task<ActionResult> Resend(string hashedAccountId, string email)
        {
            var response = await _employerTeamOrchestrator.Resend(email, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            
            return View("ViewTeam", response);
        }

        [HttpGet]
        [Route("{email}/remove/")]
        public async Task<ActionResult> Remove(string hashedAccountId, string email)
        {
            var response = await _employerTeamOrchestrator.Review(hashedAccountId, email);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{email}/remove")]
        public async Task<ActionResult> Remove(long userId, string hashedAccountId, string email, int remove)
        {
            Exception exception;
            HttpStatusCode httpStatusCode;
            
            try
            {
                if (remove != 1)
                    return RedirectToAction("ViewTeam", new { HashedAccountId = hashedAccountId });

                var response = await _employerTeamOrchestrator.Remove(userId, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                
                return View("ViewTeam", response);
            }
            catch (InvalidRequestException e)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
                exception = e;
            }
            catch (UnauthorizedAccessException e)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                exception = e;
            }

            var errorResponse = await _employerTeamOrchestrator.Review(hashedAccountId, email);
            errorResponse.Status = httpStatusCode;
            errorResponse.Exception = exception;

            return View(errorResponse);
        }

        [HttpGet]
        [Route("{email}/role/change")]
        public async Task<ActionResult> ChangeRole(string hashedAccountId, string email)
        {
            var teamMember = await _employerTeamOrchestrator.GetTeamMember(hashedAccountId, email, OwinWrapper.GetClaimValue(@"sub"));

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{email}/role/change")]
        public async Task<ActionResult> ChangeRole(string hashedAccountId, string email, short role)
        {
            var response = await _employerTeamOrchestrator.ChangeRole(hashedAccountId, email, role, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                return View("ViewTeam", response);
            }
            
            var teamMemberResponse = await _employerTeamOrchestrator.GetTeamMember(hashedAccountId, email, OwinWrapper.GetClaimValue(@"sub"));

            //We have to override flash message as the change role view has different model to view team view
            teamMemberResponse.FlashMessage = response.FlashMessage;
            teamMemberResponse.Exception = response.Exception;

            return View(teamMemberResponse);
        }

        [HttpGet]
        [Route("{email}/review/")]
        public async Task<ActionResult> Review(string hashedAccountId, string email)
        {
            var invitation = await _employerTeamOrchestrator.GetTeamMember(hashedAccountId, email, OwinWrapper.GetClaimValue(@"sub"));

            return View(invitation);
        }
    }
}