using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly HomeOrchestrator _homeOrchestrator;

        public HomeController(IOwinWrapper owinWrapper, HomeOrchestrator homeOrchestrator)
        {
            _owinWrapper = owinWrapper;
            _homeOrchestrator = homeOrchestrator;
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {

            var accounts = await _homeOrchestrator.GetUserAccounts();

            return View(accounts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SignInUser(SignInUserViewModel model)
        {
            var selectedUser = this.Request.Params["SelectedUserId"];
            var selected = model.AvailableUsers.FirstOrDefault(x => selectedUser == x.UserId);

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



        private void LoginUser(string id, string firstName, string lastName)
        {
            var displayName = $"{firstName} {lastName}";
            _owinWrapper.SignInUser(id,displayName,$"{firstName}.{lastName}@local.test");

            _owinWrapper.IssueLoginCookie(id, displayName);

            _owinWrapper.RemovePartialLoginCookie();
        }

        public ActionResult SignOut()
        {
            _owinWrapper.SignOutUser();

            return RedirectToAction("Index");
        }
    }
}