using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly HomeOrchestrator _homeOrchestrator;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public HomeController(IOwinWrapper owinWrapper, HomeOrchestrator homeOrchestrator,
            EmployerApprenticeshipsServiceConfiguration configuration, IFeatureToggle featureToggle, IUserWhiteList userWhiteList)
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            _homeOrchestrator = homeOrchestrator;
            _configuration = configuration;
        }

        
        public async Task<ActionResult> Index()
        {
            var userId = OwinWrapper.GetClaimValue("sub");
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var accounts = await _homeOrchestrator.GetUserAccounts(userId);

                if (accounts.Data.Accounts?.AccountList != null && accounts.Data.Accounts.AccountList.Count == 0)
                {
                    TempData["HideBreadcrumb"] = true;
                    return RedirectToAction("SelectEmployer", "EmployerAccount");
                }

                if (!string.IsNullOrEmpty(TempData["FlashMessage"]?.ToString()))
                {
                    accounts.FlashMessage = JsonConvert.DeserializeObject<FlashMessageViewModel>(TempData["FlashMessage"].ToString());
                }
                else
                {
                    accounts.Data.ErrorMessage = (string)TempData["errorMessage"];
                    accounts.Data.FlashMessage = new FlashMessageViewModel
                    {
                        Headline = (string)TempData["successMessage"]
                    };
                }
                

                var c = new Constants(_configuration.Identity);
                ViewBag.ChangePasswordLink = $"{c.ChangePasswordLink()}?returnurl={Url?.Encode( Request?.Url?.AbsoluteUri + "Home/HandlePasswordChanged")}";
                ViewBag.ChangeEmailLink = $"{c.ChangeEmailLink()}?returnurl={Url?.Encode(Request?.Url?.AbsoluteUri + "Home/HandleEmailChanged")}"; 
                
                return View(accounts);
            }

            var model = new
            {
                HideHeaderSignInLink = true
               
            };

            return View("UsedServiceBefore", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UsedServiceBefore(int? choice)
        {
            switch (choice ?? 0)
            {
                case 1: return RedirectToAction("WhatYoullNeed"); //No I have not used the service before
                case 2: return RedirectToAction("SignIn"); // Yes I have used the service
                default:
                    TempData["Error"] = "You must select an option to continue.";

                    var model = new
                    {
                        HideHeaderSignInLink = true
                    };

                    return View(model); //No option entered
            }
        }

        [HttpGet]
        public ActionResult WhatYoullNeed()
        {
            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult WhatYoullNeed(int? choice)
        {
            return RedirectToAction("RegisterUser");
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            var schema = System.Web.HttpContext.Current.Request.Url.Scheme;
            var authority = System.Web.HttpContext.Current.Request.Url.Authority;

            return new RedirectResult($"{_configuration.Identity.BaseAddress}/Login/dialog/appl/selfcare/wflow/register?sfaredirecturl={schema}://{authority}/Home/HandleNewRegistration");
        }

        [Authorize]
        [HttpGet]
        public ActionResult HandleNewRegistration()
        {
            TempData["successMessage"] = @"You've created your profile";
            TempData["virtualPageUrl"] = @"/user-created-account";
            TempData["virtualPageTitle"] = @"User Action - Created Account";

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult HandlePasswordChanged()
        {
            TempData["successMessage"] = @"You've changed your password";
            TempData["virtualPageUrl"] = @"/user-changed-password";
            TempData["virtualPageTitle"] = @"User Action - Changed Password";

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult HandleEmailChanged()
        {
            TempData["successMessage"] = @"You've changed your email";
            TempData["virtualPageUrl"] = @"/user-changed-email";
            TempData["virtualPageTitle"] = @"User Action - Changed Email";

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult SignIn()
        {
            return RedirectToAction("Index");
        }
        
        public ActionResult SignOut()
        {
            return OwinWrapper.SignOutUser();
        }

        [HttpGet]
        public ActionResult Privacy()
        {
            return View();
        }

        
    }
}