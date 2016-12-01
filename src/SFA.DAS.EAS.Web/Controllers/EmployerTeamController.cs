using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
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
        public async Task<ActionResult> Index(string HashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");

            var response = await _employerTeamOrchestrator.GetAccount(HashedAccountId, userIdClaim);

            return View(response);
      
        }

        [HttpGet]
        [Route("Teams")]
        public async Task<ActionResult> ViewTeam(string HashedAccountId)
        {
            var response = await _employerTeamOrchestrator.GetTeamMembers(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("Teams/Invite")]
        public async Task<ActionResult> Invite(string HashedAccountId)
        {
            var response = await _employerTeamOrchestrator.GetNewInvitation(HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/Invite")]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var response = await _employerTeamOrchestrator.InviteTeamMember(model, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                TempData["userAdded"] = "true";
                return View("ViewTeam", response);
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
        [Route("Teams/{invitationId}/Cancel")]
        public async Task<ActionResult> Cancel(string email, string invitationId, string HashedAccountId)
        {
            var invitation = await _employerTeamOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{invitationId}/Cancel")]
        public async Task<ActionResult> Cancel(string invitationId, string email, string HashedAccountId, int cancel)
        {
            if (cancel != 1)
                return RedirectToAction("ViewTeam", new { HashedAccountId });

            var response =  await _employerTeamOrchestrator.Cancel(email, HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View("ViewTeam", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/Resend")]
        public async Task<ActionResult> Resend(string HashedAccountId, string email)
        {
            var response = await _employerTeamOrchestrator.Resend(email, HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            
            return View("ViewTeam", response);
        }

        [HttpGet]
        [Route("Teams/{email}/Remove/")]
        public async Task<ActionResult> Remove(string HashedAccountId, string email)
        {
            var response = await _employerTeamOrchestrator.Review(HashedAccountId, email);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{email}/Remove")]
        public async Task<ActionResult> Remove(long userId, string HashedAccountId, string email, int remove)
        {
            Exception exception;
            HttpStatusCode httpStatusCode;
            
            try
            {
                if (remove != 1)
                    return RedirectToAction("ViewTeam", new { HashedAccountId });

                var response = await _employerTeamOrchestrator.Remove(userId, HashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                TempData["userDeleted"] = "true";
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

            var errorResponse = await _employerTeamOrchestrator.Review(HashedAccountId, email);
            errorResponse.Status = httpStatusCode;
            errorResponse.Exception = exception;

            return View(errorResponse);
        }

        [HttpGet]
        [Route("Teams/{email}/ChangeRole/")]
        public async Task<ActionResult> ChangeRole(string HashedAccountId, string email)
        {
            var teamMember = await _employerTeamOrchestrator.GetTeamMember(HashedAccountId, email, OwinWrapper.GetClaimValue(@"sub"));

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{email}/ChangeRole")]
        public async Task<ActionResult> ChangeRole(string HashedAccountId, string email, short role)
        {
            var response = await _employerTeamOrchestrator.ChangeRole(HashedAccountId, email, role, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                TempData["userRoleChange"] = "true";
                return View("ViewTeam", response);
            }
            
            var teamMemberResponse = await _employerTeamOrchestrator.GetTeamMember(HashedAccountId, email, OwinWrapper.GetClaimValue(@"sub"));

            //We have to override flash message as the change role view has different model to view team view
            teamMemberResponse.FlashMessage = response.FlashMessage;
            teamMemberResponse.Exception = response.Exception;

            return View(teamMemberResponse);
        }

        [HttpGet]
        [Route("Teams/{email}/Review/")]
        public async Task<ActionResult> Review(string HashedAccountId, string email)
        {
            var invitation = await _employerTeamOrchestrator.GetTeamMember(HashedAccountId, email, OwinWrapper.GetClaimValue(@"sub"));

            return View(invitation);
        }
    }
}