using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly HomeOrchestrator _homeOrchestrator;

        public HomeController(IOwinWrapper owinWrapper, HomeOrchestrator homeOrchestrator)
        {
            _owinWrapper = owinWrapper;
            _homeOrchestrator = homeOrchestrator;
        }
        
        public async Task<ActionResult> Index()
        {
            var userId = _owinWrapper.GetClaimValue("sub");
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var accounts = await _homeOrchestrator.GetUserAccounts(userId);

                accounts.Data.ErrorMessage = (string)TempData["errorMessage"];

                return View(accounts);
            }
            
            return View("ServiceLandingPage");
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
            return _owinWrapper.SignOutUser();

        }

        private void LoginUser(string id, string firstName, string lastName)
        {
            var displayName = $"{firstName} {lastName}";
            _owinWrapper.SignInUser(id, displayName, $"{firstName.Trim()}.{lastName.Trim()}@test.local");

            _owinWrapper.IssueLoginCookie(id, displayName);

            _owinWrapper.RemovePartialLoginCookie();
        }

       
    }
}