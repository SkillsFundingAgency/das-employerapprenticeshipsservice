using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
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

                accounts.Data.ErrorMessage = (string)TempData["errorMessage"];
                accounts.Data.FlashMessage = new FlashMessageViewModel()
                {
                    Headline = (string)TempData["successMessage"]
                };
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

        private void LoginUser(string id, string firstName, string lastName)
        {
            var displayName = $"{firstName} {lastName}";
            OwinWrapper.SignInUser(id, displayName, $"{firstName.Trim()}.{lastName.Trim()}@test.local");

            OwinWrapper.IssueLoginCookie(id, displayName);

            OwinWrapper.RemovePartialLoginCookie();
        }


    }
}