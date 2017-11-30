using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("service")]
    public class HomeController : BaseController
    {
        private readonly HomeOrchestrator _homeOrchestrator;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public HomeController(IOwinWrapper owinWrapper, HomeOrchestrator homeOrchestrator,
            EmployerApprenticeshipsServiceConfiguration configuration, IFeatureToggle featureToggle, 
            IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _homeOrchestrator = homeOrchestrator;
            _configuration = configuration;
        }

        [Route("~/")]
        [Route]
        [Route("Index")]
        public async Task<ActionResult> Index()
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName);
            if (!string.IsNullOrWhiteSpace(userId))
            {

                var partialLogin = OwinWrapper.GetClaimValue(DasClaimTypes.RequiresVerification);

                if (partialLogin.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    return Redirect(ConfigurationFactory.Current.Get().AccountActivationUrl);
                }

                var accounts = await _homeOrchestrator.GetUserAccounts(userId);

                if (accounts.Data.Accounts.AccountList.Count == 1)
                {
                    var account = accounts.Data.Accounts.AccountList.FirstOrDefault();
                    return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { HashedAccountId = account.HashedId });
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
                
                return View(ControllerConstants.SetupAccountViewName, accounts);

            }

            var model = new
            {
                HideHeaderSignInLink = true

            };

            return View(ControllerConstants.ServiceStartPageViewName, model);
        }

        [AuthoriseActiveUser]
        [HttpGet]
        [Route("accounts")]
        public async Task<ActionResult> ViewAccounts()
        {

            var accounts = await _homeOrchestrator.GetUserAccounts(OwinWrapper.GetClaimValue("sub"));

            return View(ControllerConstants.IndexActionName, accounts);
        }

        [HttpGet]
        [Route("usedServiceBefore")]
        public ActionResult UsedServiceBefore()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("usedServiceBefore")]
        public ActionResult UsedServiceBefore(int? choice)
        {
            switch (choice ?? 0)
            {
                case 1: return RedirectToAction(ControllerConstants.WhatYoullNeedActionName); //No I have not used the service before
                case 2: return RedirectToAction(ControllerConstants.SignInActionName); // Yes I have used the service
                default:
                    
                    var model = new
                    {
                        HideHeaderSignInLink = true,
                        ErrorMessage = "You must select an option to continue."
                    };

                    return View(model); //No option entered
            }
        }

        [HttpGet]
        [Route("whatYoullNeed")]
        public ActionResult WhatYoullNeed()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(model);
        }

        [HttpPost]
        [Route("whatYoullNeed")]
        public ActionResult WhatYoullNeed(int? choice)
        {
            return RedirectToAction(ControllerConstants.RegisterUserActionName);
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
        public ActionResult HandleNewRegistration()
        {
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

                var userRef = OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName);
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
            return OwinWrapper.SignOutUser();
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