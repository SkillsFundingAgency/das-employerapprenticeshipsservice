using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;
        private readonly IMediator _mediator;

        public ErrorController(
            IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService, 
            ICookieStorageService<FlashMessageViewModel> flashMessage, IAccountApiClient accountApiClient, EmployerTeamOrchestrator employerTeamOrchestrator, IMediator mediator) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _accountApiClient = accountApiClient;
            _employerTeamOrchestrator = employerTeamOrchestrator;
            _mediator = mediator;
        }

        [DasAuthorize]
        [Route("error/accessdenied/{HashedAccountId}")]
        public async Task<ActionResult> AccessDenied(string hashedAccountId)
        {
            var response = new OrchestratorResponse<AccessDeniedViewModel>();
            var queryResponse = await _mediator.SendAsync(new GetMemberRequest
            {
                HashedAccountId = hashedAccountId
                //Email = email
            });

            response.Data = MapFrom(queryResponse.TeamMember);

            ViewBag.AccountId = hashedAccountId;
            return View(response);
        }

        [ChildActionOnly]
        public override ActionResult SupportUserBanner(IAccountIdentifier model = null)
        {
            EmployerAccounts.Models.Account.Account account = null;

            if (model != null && model.HashedAccountId != null)
            {
                var response = AsyncHelper.RunSync(() => GetAccountInformation(model.HashedAccountId));

                account = response.Status != HttpStatusCode.OK ? null : response.Data.Account;
            }

            return PartialView("_SupportUserBanner", new SupportUserBannerViewModel() { Account = account });
        }

        private static AccessDeniedViewModel MapFrom(TeamMember teamMember)
        {
            return new AccessDeniedViewModel
            {
                IsUser = teamMember.IsUser,
                Id = teamMember.Id,
                AccountId = teamMember.AccountId,
                Email = teamMember.Email,
                Name = teamMember.Name,
                HashedAccountId = teamMember.HashedAccountId
            };
        }

        private async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccountInformation(string hashedAccountId)
        {
            var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var response = await _employerTeamOrchestrator.GetAccount(hashedAccountId, externalUserId);

            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage != null)
            {
                response.FlashMessage = flashMessage;
                response.Data.EmployerAccountType = flashMessage.HiddenFlashMessageInformation;
            }

            return response;
        }


        [Route("accessdenied")]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View();
        }

        [Route("error")]
        public ActionResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return View();
        }

        [Route("notfound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }      
    }
}