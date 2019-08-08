﻿using SFA.DAS.Authentication;
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
using System.Web.Routing;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.NLog.Logger;

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
        public async Task<ActionResult> Index(string returnUrl = "")
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await OwinWrapper.UpdateClaims();

                var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                var email = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);
                var firstName = OwinWrapper.GetClaimValue(DasClaimTypes.GivenName);
                var lastName = OwinWrapper.GetClaimValue(DasClaimTypes.FamilyName);

                await _homeOrchestrator.SaveUpdatedIdentityAttributes(userRef, email, firstName, lastName); //todo here user is created in accounts db

                var partialLogin = OwinWrapper.GetClaimValue(DasClaimTypes.RequiresVerification);

                if (partialLogin.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    return Redirect(ConfigurationFactory.Current.Get().AccountActivationUrl);
                }

                var accounts = await _homeOrchestrator.GetUserAccounts(userId);

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

                return RedirectToAction(ControllerConstants.GetApprenticeshipFundingActionName, ControllerConstants.EmployerAccountControllerName, new { returnUrl });
            }

            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(ControllerConstants.ServiceStartPageViewName, model);
        }

        [Authorize]
        [Route("SaveAndSearch")]
        public async Task<ActionResult> SaveAndSearch(string returnUrl)
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

        [AuthoriseActiveUser]
        [HttpGet]
        [Route("accounts")]
        public async Task<ActionResult> ViewAccounts()
        {
            var accounts = await _homeOrchestrator.GetUserAccounts(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
            return View(ControllerConstants.IndexActionName, accounts);
        }     
     
        [HttpGet]
        [Route("register")]
        public ActionResult RegisterUser()
        {
            var schema = System.Web.HttpContext.Current.Request.Url.Scheme;
            var authority = System.Web.HttpContext.Current.Request.Url.Authority;
            var c = new Constants(_configuration.Identity);
            return new RedirectResult($"{c.RegisterLink()}{schema}://{authority}/service/register/new");
        }

        [Authorize]
        [HttpGet]
        [Route("register/new")]
        public async Task<ActionResult> HandleNewRegistration()
        {
            await OwinWrapper.UpdateClaims();
            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [Authorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
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

        [Authorize]
        [HttpGet]
        [Route("email/change")]
        public async Task<ActionResult> HandleEmailChanged(bool userCancelled = false)
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

        [Authorize]
        [Route("signIn")]
        public ActionResult SignIn()
        {
            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            OwinWrapper.SignOutUser();

            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken));
        }

        [Route("SignOutCleanup")]
        public void SignOutCleanup()
        {
            OwinWrapper.SignOutUser();
        }

        [HttpGet]
        [Route("privacy")]
        public ActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("help")]
        public ActionResult Help()
        {
            return View();
        }

        [HttpGet]
        [Route("start")]
        public ActionResult ServiceStartPage()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(model);
        }

#if DEBUG
        [Route("CreateLegalAgreement/{showSubFields}")]
        public ActionResult ShowLegalAgreement(bool showSubFields)
        {
            return View(ControllerConstants.LegalAgreementViewName, showSubFields);
        }
#endif
    }
}