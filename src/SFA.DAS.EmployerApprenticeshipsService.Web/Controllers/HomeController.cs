using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOwinWrapper _owinWrapper;

        public HomeController(IOwinWrapper owinWrapper)
        {
            _owinWrapper = owinWrapper;
        }

        public ActionResult Index()
        {
            return View();
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
        public ActionResult SignInUser(SignInUserModel model)
        {
            LoginUser(model.UserId,model.FirstName,model.LastName);

            return RedirectToAction("Index");
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