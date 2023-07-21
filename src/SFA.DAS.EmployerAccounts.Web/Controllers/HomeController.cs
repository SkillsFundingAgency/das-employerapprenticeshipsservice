﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("service")]
public class HomeController : BaseController
{
    private readonly HomeOrchestrator _homeOrchestrator;
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly ICookieStorageService<ReturnUrlModel> _returnUrlCookieStorageService;
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;
    private readonly IStubAuthenticationService _stubAuthenticationService;
    private readonly IUrlActionHelper _urlHelper;

    public const string ReturnUrlCookieName = "SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie";

    public HomeController(
        HomeOrchestrator homeOrchestrator,
        EmployerAccountsConfiguration configuration,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        ICookieStorageService<ReturnUrlModel> returnUrlCookieStorageService,
        ILogger<HomeController> logger,
        IConfiguration config,
        IStubAuthenticationService stubAuthenticationService, IUrlActionHelper urlHelper)
        : base(flashMessage)
    {
        _homeOrchestrator = homeOrchestrator;
        _configuration = configuration;
        _returnUrlCookieStorageService = returnUrlCookieStorageService;
        _logger = logger;
        _config = config;
        _stubAuthenticationService = stubAuthenticationService;
        _urlHelper = urlHelper;
    }

    [Route("~/")]
    [Route("Index")]
    public async Task<IActionResult> Index(GaQueryData queryData)
    {
        // check if the GovSignIn is enabled
      if (_configuration.UseGovSignIn)
        {
            if (User.Identities.FirstOrDefault() != null && User.Identities.FirstOrDefault()!.IsAuthenticated)
            {
                var userRef = HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier);
                
                var userDetail = await _homeOrchestrator.GetUser(userRef);
                
                if (userDetail == null || string.IsNullOrEmpty(userDetail.FirstName) || string.IsNullOrEmpty(userDetail.LastName) || string.IsNullOrEmpty(userRef))
                {
                    return Redirect(_urlHelper.EmployerProfileAddUserDetails($"/user/add-user-details") + $"?_ga={queryData._ga}&_gl={queryData._gl}&utm_source={queryData.utm_source}&utm_campaign={queryData.utm_campaign}&utm_medium={queryData.utm_medium}&utm_keywords={queryData.utm_keywords}&utm_content={queryData.utm_content}");    
                }
            }
        }
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ControllerConstants.UserRefClaimKeyName));

        OrchestratorResponse<UserAccountsViewModel> accounts;

        if (userIdClaim != null)
        {
            if (!_configuration.UseGovSignIn)
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
            }

            accounts = await _homeOrchestrator.GetUserAccounts(userIdClaim.Value, _configuration.LastTermsAndConditionsUpdate);
        }
        else
        {
            if (_config["ResourceEnvironmentName"].Equals("PROD"))
            {
                //GDS requirement that users begin their service journey on .gov.uk
                return Redirect(_configuration.GovUkSignInToASAccountUrl);
            }

            var model = new ServiceStartPageViewModel
            {
                HideHeaderSignInLink = true,
                GovSignInEnabled = _configuration.UseGovSignIn
            };

            return View(ControllerConstants.ServiceStartPageViewName, model);
        }



        if (accounts.Data.Invitations > 0)
        {
            return RedirectToAction(ControllerConstants.InvitationIndexName, ControllerConstants.InvitationControllerName,queryData);
        }

        // condition to check if the user has only one account, then redirect to home page/dashboard.
        if (accounts.Data.Accounts.AccountList.Count == 1)
        {
            var account = accounts.Data.Accounts.AccountList.FirstOrDefault();

            if (account != null)
            {
                if (account.NameConfirmed)
                {
                    return RedirectToRoute(RouteNames.EmployerTeamIndex, new
                    {
                        HashedAccountId = account.HashedId,
                        queryData._ga,
                        queryData._gl,
                        queryData.utm_source,
                        queryData.utm_campaign,
                        queryData.utm_medium,
                        queryData.utm_keywords,
                        queryData.utm_content
                    });
                } else
                {
                    return RedirectToRoute(RouteNames.ContinueNewEmployerAccountTaskList, new { hashedAccountId = account.HashedId });
                }
            }
        }

        var flashMessage = GetFlashMessageViewModelFromCookie();

        if (flashMessage != null)
        {
            accounts.FlashMessage = flashMessage;
        }

        // condition to check if the user has more than one account, then redirect to accounts page.
        if (accounts.Data.Accounts.AccountList.Count > 1)
        {
            return View(accounts);
        }

        return RedirectToRoute(RouteNames.NewEmpoyerAccountTaskList, queryData);
    }

    [Authorize]
    public IActionResult GovSignIn(GaQueryData queryData)
    {
        return RedirectToAction(ControllerConstants.IndexActionName, "Home", queryData);
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
        if (!_configuration.UseGovSignIn && !string.IsNullOrWhiteSpace(correlationId))
        {
            var userRef = HttpContext.User.FindFirstValue(ControllerConstants.UserRefClaimKeyName);
            var email = HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserEmailClaimTypeIdentifier);
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
            return _configuration.UseGovSignIn 
                ? Redirect(_urlHelper.EmployerProfileAddUserDetails($"/user/add-user-details")) 
                : new RedirectResult($"{appConstants.RegisterLink()}{schema}://{authority}/service/register/new");
        }

        var invitation = await _homeOrchestrator.GetProviderInvitation(correlationId.Value);

        if (_configuration.UseGovSignIn)
        {
            var queryData = invitation.Data != null
                ? $"?correlationId={correlationId}&firstname={WebUtility.UrlEncode(invitation.Data.EmployerFirstName)}&lastname={WebUtility.UrlEncode(invitation.Data.EmployerLastName)}"
                : "";
            return Redirect(_urlHelper.EmployerProfileAddUserDetails($"/user/add-user-details") + queryData);
        }
        
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
            var email = HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserEmailClaimTypeIdentifier);
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
   

    [Route("signOut", Name = RouteNames.SignOut)]
    public async Task<IActionResult> SignOutUser()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token", idToken);
        if (_configuration.UseGovSignIn)
        {
            var schemes = new List<string>
            {
                CookieAuthenticationDefaults.AuthenticationScheme
            };
            _ = bool.TryParse(_config["StubAuth"], out var stubAuth);
            if (!stubAuth)
            {
                schemes.Add(OpenIdConnectDefaults.AuthenticationScheme);
            }
            
            return SignOut(authenticationProperties, schemes.ToArray());    
        }
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/", Parameters = { { "id_token", idToken } } }); SignOut(authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        
        var constants = new Constants(_configuration.Identity);
        
        return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken));
    }

    [Route("signoutcleanup")]
    public async Task SignOutCleanup()
    {
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
        var model = new ServiceStartPageViewModel
        {
            HideHeaderSignInLink = true,
            GovSignInEnabled = _configuration.UseGovSignIn
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
    
    [HttpGet]
    [Route("SignIn-Stub")]
    public IActionResult SigninStub()
    {
        return View("SigninStub", new List<string>{_config["StubId"],_config["StubEmail"]});
    }
    [HttpPost]
    [Route("SignIn-Stub")]
    public async Task<IActionResult> SigninStubPost()
    {
        var claims = await _stubAuthenticationService.GetStubSignInClaims(new StubAuthUserDetails
        {
            Email = _config["StubEmail"],
            Id = _config["StubId"]
        });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
            new AuthenticationProperties());

        return RedirectToRoute("Signed-in-stub");
    }

    [Authorize]
    [HttpGet]
    [Route("signed-in-stub", Name = "Signed-in-stub")]
    public IActionResult SignedInStub()
    {
        return View();
    }
#endif
}