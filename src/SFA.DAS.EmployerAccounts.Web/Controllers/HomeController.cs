using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerAccounts.Web;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("service")]
    public class HomeController : BaseController
    {
        private readonly HomeOrchestrator _homeOrchestrator;
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly ICookieStorageService<ReturnUrlModel> _returnUrlCookieStorageService;
        private readonly ILog _logger;

        private const string ReturnUrlCookieName = "SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie";

        public HomeController(IAuthenticationService owinWrapper,
            HomeOrchestrator homeOrchestrator,
            EmployerAccountsConfiguration configuration,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            ICookieStorageService<ReturnUrlModel> returnUrlCookieStorageService,
            ILog logger)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _homeOrchestrator = homeOrchestrator;
            _configuration = configuration;
            _returnUrlCookieStorageService = returnUrlCookieStorageService;
            _logger = logger;
        }

        [Route("~/")]
        [Route]
        [Route("Index")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            OrchestratorResponse<UserAccountsViewModel> accounts;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await OwinWrapper.UpdateClaims();

                var partialLogin = OwinWrapper.GetClaimValue(DasClaimTypes.RequiresVerification);

                if (partialLogin.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    return Redirect(ConfigurationFactory.Current.Get().AccountActivationUrl);
                }
                
                var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
                var firstName = OwinWrapper.GetClaimValue(DasClaimTypes.GivenName);
                var lastName = OwinWrapper.GetClaimValue(DasClaimTypes.FamilyName);

                await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName);

                accounts = await _homeOrchestrator.GetUserAccounts(userId, _configuration.LastTermsAndConditionsUpdate);
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
        public Microsoft.AspNetCore.Mvc.ActionResult TermsAndConditionsOverview()
        {
            return View();
        }


        [HttpGet]
        [DasAuthorize]
        [Route("termsAndConditions")]
        public Microsoft.AspNetCore.Mvc.ActionResult TermsAndConditions(string returnUrl, string hashedAccountId)
        {
            var termsAndConditionsNewViewModel = new TermsAndConditionsNewViewModel { ReturnUrl = returnUrl, HashedAccountId = hashedAccountId };
            return View(termsAndConditionsNewViewModel);
        }

        [HttpPost]
        [DasAuthorize]
        [Route("termsAndConditions")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> TermsAndConditions(TermsAndConditionsNewViewModel termsAndConditionViewModel)
        {
            var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            await _homeOrchestrator.UpdateTermAndConditionsAcceptedOn(userRef);

            if (termsAndConditionViewModel.ReturnUrl == "EmployerTeam")
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { termsAndConditionViewModel.HashedAccountId });
            }
            return RedirectToAction(nameof(Index));
        }

        [DasAuthorize]
        [Route("SaveAndSearch")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> SaveAndSearch(string returnUrl)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.Warn($"UserId not found on OwinWrapper. Redirecting back to passed in returnUrl: {returnUrl}");
                return Redirect(returnUrl);
            }

            await OwinWrapper.UpdateClaims();

            var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
            var firstName = OwinWrapper.GetClaimValue(DasClaimTypes.GivenName);
            var lastName = OwinWrapper.GetClaimValue(DasClaimTypes.FamilyName);

            await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName);

            _returnUrlCookieStorageService.Create(new ReturnUrlModel { Value = returnUrl }, ReturnUrlCookieName);

            return RedirectToAction(ControllerConstants.GetApprenticeshipFundingActionName, ControllerConstants.EmployerAccountControllerName);
        }

        [DasAuthorize]
        [HttpGet]
        [Route("accounts")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ViewAccounts()
        {
            var accounts = await _homeOrchestrator.GetUserAccounts(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
            return View(ControllerConstants.IndexActionName, accounts);
        }

        [DasAuthorize]
        [HttpGet]
        [Route("register/new/{correlationId?}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> HandleNewRegistration(string correlationId = null)
        {
            await OwinWrapper.UpdateClaims();

            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
                var firstName = OwinWrapper.GetClaimValue(DasClaimTypes.GivenName);
                var lastName = OwinWrapper.GetClaimValue(DasClaimTypes.FamilyName);

                await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName, correlationId);
            }

            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [HttpGet]
        [Route("register")]
        [Route("register/{correlationId}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RegisterUser(Guid? correlationId)
        {
            var schema = HttpContextHelper.Current.Request.Url.Scheme;
            var authority = HttpContextHelper.Current.Request.Url.Authority;
            var c = new Constants(_configuration.Identity);

            if (!correlationId.HasValue) return new Microsoft.AspNetCore.Mvc.RedirectResult($"{c.RegisterLink()}{schema}://{authority}/service/register/new");

            var invitation = await _homeOrchestrator.GetProviderInvitation(correlationId.Value);

            return invitation.Data != null
                ? new Microsoft.AspNetCore.Mvc.RedirectResult($"{c.RegisterLink()}{schema}://{authority}/service/register/new/{correlationId}&firstname={Url.Encode(invitation.Data.EmployerFirstName)}&lastname={Url.Encode(invitation.Data.EmployerLastName)}&email={Url.Encode(invitation.Data.EmployerEmail)}")
                : new Microsoft.AspNetCore.Mvc.RedirectResult($"{c.RegisterLink()}{schema}://{authority}/service/register/new");
        }

        [DasAuthorize]
        [HttpGet]
        [Route("password/change")]
        public Microsoft.AspNetCore.Mvc.ActionResult HandlePasswordChanged(bool userCancelled = false)
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

        [DasAuthorize]
        [HttpGet]
        [Route("email/change")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> HandleEmailChanged(bool userCancelled = false)
        {
            if (!userCancelled)
            {
                var flashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "You've changed your email"
                };
                AddFlashMessageToCookie(flashMessage);

                await OwinWrapper.UpdateClaims();

                var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
                var firstName = OwinWrapper.GetClaimValue(DasClaimTypes.GivenName);
                var lastName = OwinWrapper.GetClaimValue(DasClaimTypes.FamilyName);

                await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName);
            }

            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [DasAuthorize]
        [Route("signIn")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignIn()
        {
            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [Route("signOut")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignOut()
        {
            OwinWrapper.SignOutUser();

            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            return new Microsoft.AspNetCore.Mvc.RedirectResult(string.Format(constants.LogoutEndpoint(), idToken));
        }

        [Route("SignOutCleanup")]
        public void SignOutCleanup()
        {
            OwinWrapper.SignOutUser();
        }

        [HttpGet]
        [Route("{HashedAccountId}/privacy", Order = 0)]
        [Route("privacy", Order = 1)]
        public Microsoft.AspNetCore.Mvc.ActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("help")]
        public Microsoft.AspNetCore.Mvc.ActionResult Help()
        {
            return RedirectPermanent(_configuration.ZenDeskHelpCentreUrl);
        }

        [HttpGet]
        [Route("start")]
        public Microsoft.AspNetCore.Mvc.ActionResult ServiceStartPage()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(model);
        }

        [HttpGet]
        [Route("unsubscribe/{correlationId}")]
        public Microsoft.AspNetCore.Mvc.ActionResult Unsubscribe()
        {
            return View();
        }

        [HttpPost]
        [Route("unsubscribe/{correlationId}")]
        [ValidateAntiForgeryToken]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Unsubscribe(bool? unsubscribe, string correlationId)
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
        public Microsoft.AspNetCore.Mvc.ActionResult ShowLegalAgreement(bool showSubFields) //call this  with false
        {
            return View(ControllerConstants.LegalAgreementViewName, showSubFields);
        }
#endif
    }
}