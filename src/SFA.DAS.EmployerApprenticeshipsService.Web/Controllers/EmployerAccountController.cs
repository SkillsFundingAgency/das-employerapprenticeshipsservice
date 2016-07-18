using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountController : Controller
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;

        public EmployerAccountController(EmployerAccountOrchestrator employerAccountOrchestrator)
        {
            if (employerAccountOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountOrchestrator));
            _employerAccountOrchestrator = employerAccountOrchestrator;
        }

        // GET: EmployerAccount
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(bool accepted)
        {
            return RedirectToAction("GovernmentGatewayConfirm");
        }

        [HttpGet]
        public ActionResult GovernmentGatewayConfirm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SelectEmployer()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SelectEmployer(SelectEmployerModel model)
        {
            var response = await _employerAccountOrchestrator.GetCompanyDetails(model);

            if (string.IsNullOrWhiteSpace(response.CompanyNumber))
                return View();

            return RedirectToAction("VerifyEmployer", response);
        }

        [HttpGet]
        public ActionResult VerifyEmployer(SelectEmployerViewModel model)
        {
            return View(model);
        }
    }
}