using System;
using System.Net;
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
            var response = new OrchestratorResponse<InviteTeamMemberViewModel>
            {
                Data = new InviteTeamMemberViewModel
                {
                    HashedId = accountId,
                    Role = Role.Viewer
                }
            };

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/Invite")]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var response = await _employerTeamOrchestrator.InviteTeamMember(model, OwinWrapper.GetClaimValue(@"sub"));

            if(response.Status == HttpStatusCode.OK)
                return View("ViewTeam", response);
           
            var errorResponse = new OrchestratorResponse<InviteTeamMemberViewModel>
            {
                Data = model,
                Status = response.Status,
                Exception = response.Exception
            };

            return View(errorResponse);
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
            if (cancel != 1)
                return RedirectToAction("ViewTeam", new {accountId});

            var response =  await _employerTeamOrchestrator.Cancel(email, accountId, OwinWrapper.GetClaimValue(@"sub"));

            return View("ViewTeam", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/Resend")]
        public async Task<ActionResult> Resend(string accountId, string email)
        {
            var response = await _employerTeamOrchestrator.Resend(email, accountId, OwinWrapper.GetClaimValue(@"sub"));
            
            return View("ViewTeam", response);
        }

        [HttpGet]
        [Route("Teams/{email}/Remove/")]
        public async Task<ActionResult> Remove(string accountId, string email)
        {
            var response = await _employerTeamOrchestrator.Review(accountId, email);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Teams/{email}/Remove")]
        public async Task<ActionResult> Remove(long userId, string accountId, string email, int remove)
        {
            Exception exception;
            HttpStatusCode httpStatusCode;
            
            try
            {
                if (remove != 1)
                    return RedirectToAction("ViewTeam", new {accountId});

                var response = await _employerTeamOrchestrator.Remove(userId, accountId, OwinWrapper.GetClaimValue(@"sub"));

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

            var errorResponse = await _employerTeamOrchestrator.Review(accountId, email);
            errorResponse.Status = httpStatusCode;
            errorResponse.Exception = exception;

            return View(errorResponse);
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
            var response = await _employerTeamOrchestrator.ChangeRole(accountId, email, role, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Status == HttpStatusCode.OK)
            {
                return View("ViewTeam", response);
            }
            
            var teamMemberResponse = await _employerTeamOrchestrator.GetTeamMember(accountId, email);

            //We have to override flash message as the change role view has different model to view team view
            teamMemberResponse.FlashMessage = response.FlashMessage;
            teamMemberResponse.Exception = response.Exception;

            return View(teamMemberResponse);
        }


        [HttpGet]
        [Route("Teams/{email}/Review/")]
        public async Task<ActionResult> Review(string accountId, string email)
        {
            var invitation = await _employerTeamOrchestrator.GetTeamMember(accountId, email);

            return View(invitation);
        }
    }
}