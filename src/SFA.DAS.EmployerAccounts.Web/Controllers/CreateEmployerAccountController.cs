using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Route("accounts")]
    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    public class CreateEmployerAccountController : EmployerAccountController
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ICookieStorageService<ReturnUrlModel> _returnUrlCookieStorageService;

        public CreateEmployerAccountController(
            EmployerAccountOrchestrator employerAccountOrchestrator,
            ILogger<EmployerAccountController> logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMediator mediatr,
            ICookieStorageService<ReturnUrlModel> returnUrlCookieStorageService,
            ICookieStorageService<HashedAccountIdModel> accountCookieStorage,
            LinkGenerator linkGenerator)
            : base(employerAccountOrchestrator, logger, flashMessage, mediatr, returnUrlCookieStorageService, accountCookieStorage, linkGenerator)
        {
            _employerAccountOrchestrator = employerAccountOrchestrator;
            _returnUrlCookieStorageService = returnUrlCookieStorageService;
        }

        [HttpGet]
        [Route("getApprenticeshipFunding", Order = 1, Name = RouteNames.EmployerAccountGetApprenticeshipFunding)]
        new public IActionResult GetApprenticeshipFunding()
        {
            return base.GetApprenticeshipFunding();
        }

        [HttpPost]
        [Route("getApprenticeshipFunding", Order = 1, Name = RouteNames.EmployerAccountPostApprenticeshipFunding)]
        public IActionResult GetApprenticeshipFunding(int? choice)
        {
            return base.PostGetApprenticeshipFunding(choice);
        }

        [HttpGet]
        [Route("skipRegistration", Name = RouteNames.SkipRegistration)]
        public async Task<IActionResult> SkipRegistration()
        {
            var request = new CreateUserAccountViewModel
            {
                UserId = GetUserId(),
                OrganisationName = "MY ACCOUNT"
            };

            var response = await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(request, HttpContext);
            var returnUrlCookie = _returnUrlCookieStorageService.Get(ReturnUrlCookieName);

            _returnUrlCookieStorageService.Delete(ReturnUrlCookieName);

            if (returnUrlCookie != null && !string.IsNullOrWhiteSpace(returnUrlCookie.Value))
            {
                return Redirect(returnUrlCookie.Value);
            }

            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { hashedAccountId = response.Data.HashedId });
        }

        private string GetUserId()
        {
            var userIdClaim = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
            return userIdClaim ?? "";
        }
    }
}
