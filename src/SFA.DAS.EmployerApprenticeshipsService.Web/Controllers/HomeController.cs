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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SignInUser(SignInUserModel model)
        {
            LoginUser(model.UserId,model.FirstName,model.LastName);

            return View("Index");
        }

        private void LoginUser(string id, string firstName, string lastName)
        {
            _owinWrapper.IssueLoginCookie(id, $"{firstName} {lastName}");

            _owinWrapper.RemovePartialLoginCookie();
        }
    }
}