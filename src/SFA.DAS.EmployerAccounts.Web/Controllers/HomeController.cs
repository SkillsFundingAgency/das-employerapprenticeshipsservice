using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("service")]
public class HomeController : BaseController
{
    private readonly HomeOrchestrator _homeOrchestrator;
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly ICookieStorageService<ReturnUrlModel> _returnUrlCookieStorageService;
    private readonly ILogger<HomeController> _logger;

    public const string ReturnUrlCookieName = "SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie";

    public HomeController(
        HomeOrchestrator homeOrchestrator,
        EmployerAccountsConfiguration configuration,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        ICookieStorageService<ReturnUrlModel> returnUrlCookieStorageService,
        ILogger<HomeController> logger)
        : base(flashMessage)
    {
        _homeOrchestrator = homeOrchestrator;
        _configuration = configuration;
        _returnUrlCookieStorageService = returnUrlCookieStorageService;
        _logger = logger;
    }

    [Route("~/")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ControllerConstants.UserRefClaimKeyName));

        OrchestratorResponse<UserAccountsViewModel> accounts;

        if (userIdClaim != null)
        {
            var partialLogin = HttpContext.User.FindFirstValue(DasClaimTypes.RequiresVerification);

            if (partialLogin.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                return Redirect(ConfigurationFactory.Current.Get().AccountActivationUrl);
            }

            var userRef = HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier);
            var email = HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserEmailClaimTypeIdentifier);
            var firstName = HttpContext.User.FindFirstValue(DasClaimTypes.GivenName);
            var lastName = HttpContext.User.FindFirstValue(DasClaimTypes.FamilyName);

            await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName);

            accounts = await _homeOrchestrator.GetUserAccounts(userIdClaim.Value, _configuration.LastTermsAndConditionsUpdate);
        }
        else
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(ControllerConstants.ServiceStartPageViewName, model);
        }

        if (accounts.Data.Invitations > 0)
        {
            return RedirectToAction(ControllerConstants.InvitationIndexName, ControllerConstants.InvitationControllerName);
        }

        if (accounts.Data.Accounts.AccountList.Count == 1)
        {
            var account = accounts.Data.Accounts.AccountList.FirstOrDefault();

            if (account != null)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { HashedAccountId = account.HashedId });
            }
        }

        var flashMessage = GetFlashMessageViewModelFromCookie();

        if (flashMessage != null)
        {
            accounts.FlashMessage = flashMessage;
        }

        if (accounts.Data.Accounts.AccountList.Count > 1)
        {
            return View(accounts);
        }

        return RedirectToAction(ControllerConstants.GetApprenticeshipFundingActionName, ControllerConstants.EmployerAccountControllerName);
    }

    [HttpGet]
    [Route("termsAndConditions/overview")]
    public IActionResult TermsAndConditionsOverview()
    {
        return View();
    }


    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [Route("termsAndConditions")]
    public IActionResult TermsAndConditions(string returnUrl, string hashedAccountId)
    {
        var termsAndConditionsNewViewModel = new TermsAndConditionsNewViewModel { ReturnUrl = returnUrl, HashedAccountId = hashedAccountId };
        return View(termsAndConditionsNewViewModel);
    }

    [HttpPost]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [Route("termsAndConditions")]
    public async Task<IActionResult> TermsAndConditions(TermsAndConditionsNewViewModel termsAndConditionViewModel)
    {
        var userRef = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        await _homeOrchestrator.UpdateTermAndConditionsAcceptedOn(userRef);

        if (termsAndConditionViewModel.ReturnUrl == "EmployerTeam")
        {
            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { termsAndConditionViewModel.HashedAccountId });
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [Route("SaveAndSearch")]
    public async Task<IActionResult> SaveAndSearch(string returnUrl)
    {
        var userId = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning($"UserId not found on OwinWrapper. Redirecting back to passed in returnUrl: {returnUrl}");
            return Redirect(returnUrl);
        }

        var userRef = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
        var email = HttpContext.User.FindFirstValue(ControllerConstants.EmailClaimKeyName);
        var firstName = HttpContext.User.FindFirstValue(DasClaimTypes.GivenName);
        var lastName = HttpContext.User.FindFirstValue(DasClaimTypes.FamilyName);

        await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName);

        _returnUrlCookieStorageService.Create(new ReturnUrlModel { Value = returnUrl }, ReturnUrlCookieName);

        return RedirectToAction(ControllerConstants.GetApprenticeshipFundingActionName, ControllerConstants.EmployerAccountControllerName);
    }

    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [HttpGet]
    [Route("accounts")]
    public async Task<IActionResult> ViewAccounts()
    {
        var accounts = await _homeOrchestrator.GetUserAccounts(HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName));
        return View(ControllerConstants.IndexActionName, accounts);
    }

    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [HttpGet]
    [Route("register/new/{correlationId?}")]
    public async Task<IActionResult> HandleNewRegistration(string correlationId = null)
    {
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            var userRef = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
            var email = HttpContext.User.FindFirstValue(ControllerConstants.EmailClaimKeyName);
            var firstName = HttpContext.User.FindFirstValue(DasClaimTypes.GivenName);
            var lastName = HttpContext.User.FindFirstValue(DasClaimTypes.FamilyName);

            await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName, correlationId);
        }

        return RedirectToAction(ControllerConstants.IndexActionName);
    }

    [HttpGet]
    [Route("register")]
    [Route("register/{correlationId}")]
    public async Task<IActionResult> RegisterUser(Guid? correlationId)
    {
        var schema = Request.Scheme;
        var authority = HttpContext?.Request.Host.Value;
        var appConstants = new Constants(_configuration.Identity);

        if (!correlationId.HasValue)
        {
            return new RedirectResult($"{appConstants.RegisterLink()}{schema}://{authority}/service/register/new");
        }

        var invitation = await _homeOrchestrator.GetProviderInvitation(correlationId.Value);

        return invitation.Data != null
            ? new RedirectResult($"{appConstants.RegisterLink()}{schema}://{authority}/service/register/new/{correlationId}&firstname={WebUtility.UrlEncode(invitation.Data.EmployerFirstName)}&lastname={WebUtility.UrlEncode(invitation.Data.EmployerLastName)}&email={WebUtility.UrlEncode(invitation.Data.EmployerEmail)}")
            : new RedirectResult($"{appConstants.RegisterLink()}{schema}://{authority}/service/register/new");
    }

    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [HttpGet]
    [Route("password/change")]
    public IActionResult HandlePasswordChanged(bool userCancelled = false)
    {
        if (!userCancelled)
        {
            var flashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = "You've changed your password"
            };
            AddFlashMessageToCookie(flashMessage);
        }

        return RedirectToAction(ControllerConstants.IndexActionName);
    }

    [Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
    [HttpGet]
    [Route("email/change")]
    public async Task<IActionResult> HandleEmailChanged(bool userCancelled = false)
    {
        if (!userCancelled)
        {
            var flashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = "You've changed your email"
            };

            AddFlashMessageToCookie(flashMessage);

            var userRef = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
            var email = HttpContext.User.FindFirstValue(ControllerConstants.EmailClaimKeyName);
            var firstName = HttpContext.User.FindFirstValue(DasClaimTypes.GivenName);
            var lastName = HttpContext.User.FindFirstValue(DasClaimTypes.FamilyName);

            await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName);
        }

        return RedirectToAction(ControllerConstants.IndexActionName);
    }

    [Authorize]
    [Route("signIn")]
    public IActionResult SignIn()
    {
        return RedirectToAction(ControllerConstants.IndexActionName);
    }

    [Route("signOut")]
    public async Task<IActionResult> SignOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token", idToken);
        SignOut(authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

        var constants = new Constants(_configuration.Identity);

        return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken));
    }

    [Route("SignOutCleanup")]
    public async Task SignOutCleanup()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token", idToken);

        SignOut(authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet]
    [Route("{HashedAccountId}/privacy", Order = 0)]
    [Route("privacy", Order = 1)]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    [Route("help")]
    public IActionResult Help()
    {
        return RedirectPermanent(_configuration.ZenDeskHelpCentreUrl);
    }

    [HttpGet]
    [Route("start")]
    public IActionResult ServiceStartPage()
    {
        var model = new
        {
            HideHeaderSignInLink = true
        };

        return View(model);
    }

    [HttpGet]
    [Route("unsubscribe/{correlationId}")]
    public IActionResult Unsubscribe()
    {
        return View();
    }

    [HttpPost]
    [Route("unsubscribe/{correlationId}")]
    public async Task<IActionResult> Unsubscribe(bool? unsubscribe, string correlationId)
    {
        if (unsubscribe == null || unsubscribe == false)
        {
            var model = new
            {
                InError = true
            };

            return View(model);
        }

        await _homeOrchestrator.Unsubscribe(Guid.Parse(correlationId));

        return View(ControllerConstants.UnsubscribedViewName);
    }

#if DEBUG
    [Route("CreateLegalAgreement/{showSubFields}")]
    public IActionResult ShowLegalAgreement(bool showSubFields) //call this  with false
    {
        return View(ControllerConstants.LegalAgreementViewName, showSubFields);
    }
#endif
}