using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
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

                if (!string.IsNullOrEmpty(TempData["FlashMessage"]?.ToString()))
                {
                    accounts.FlashMessage = JsonConvert.DeserializeObject<FlashMessageViewModel>(TempData["FlashMessage"].ToString());
                }
                else
                {
                    accounts.Data.ErrorMessage = (string)TempData["errorMessage"];
                    accounts.Data.FlashMessage = new FlashMessageViewModel()
                    {
                        Headline = (string)TempData["successMessage"]
                    };
                }
                

                var c = new Constants(_configuration.Identity?.BaseAddress);
                ViewBag.ChangePasswordLink = $"{c.ChangePasswordLink()}?myaccount={Url?.Encode( Request?.Url?.AbsoluteUri)}";
                ViewBag.ChangeEmailLink = $"{c.ChangeEmailLink()}?myaccount={Url?.Encode(Request?.Url?.AbsoluteUri)}"; 
                
                return View(accounts);
            }

            var model = new
            {
                HideHeaderSignInLink = true
               
            };

            return View("ServiceLandingPage", model);
        }
        
        [HttpGet]
        public ActionResult RegisterUser()
        {
            var schema = System.Web.HttpContext.Current.Request.Url.Scheme;
            var authority = System.Web.HttpContext.Current.Request.Url.Authority;

            return new RedirectResult($"{_configuration.Identity.BaseAddress}/Login/dialog/appl/selfcare/wflow/register?sfaredirecturl={schema}://{authority}/Home/HandleNewRegistraion");
        }

        [Authorize]
        [HttpGet]
        public ActionResult HandleNewRegistraion()
        {
            ViewData["successMessage"] = @"You've created your profile";
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult SignIn()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SignInUser(string selectedUserId, SignInUserViewModel model)
        {

            var selected = model.AvailableUsers.FirstOrDefault(x => selectedUserId == x.UserId);

            if (selected != null)
            {
                LoginUser(selected.UserId, selected.FirstName, selected.LastName);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> FakeUserSignIn()
        {
            var users = await _homeOrchestrator.GetUsers();

            return View(users);
        }

        public ActionResult SignOut()
        {
            return OwinWrapper.SignOutUser();

        }

        [HttpGet]
        public ActionResult Privacy()
        {
            //var model = new PrivacyViewModel
            //{
            //    AboutUrl = _configuration.Privacy.AboutCookiesUrl,
            //    SurveyProviderUrl = _configuration.Privacy.SurveyProviderUrl,
            //    GoogleUrl = _configuration.Privacy.GoogleAnalyticsUrl,
            //    ApplicationInsightsUrl = _configuration.Privacy.ApplicationInsightsUrl,
            //};

            return View();
        }

        private void LoginUser(string id, string firstName, string lastName)
        {
            var displayName = $"{firstName} {lastName}";
            OwinWrapper.SignInUser(id, displayName, $"{firstName.Trim()}.{lastName.Trim()}@test.local");

            OwinWrapper.IssueLoginCookie(id, displayName);

            OwinWrapper.RemovePartialLoginCookie();
        }


    }
}