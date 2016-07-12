using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountController : Controller
    {
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ICookieService _cookieService;

        public EmployerAccountController(EmployerAccountOrchestrator employerAccountOrchestrator, ICookieService cookieService)
        {
            if (employerAccountOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountOrchestrator));
            if (cookieService == null)
                throw new ArgumentNullException(nameof(cookieService));
            _employerAccountOrchestrator = employerAccountOrchestrator;
            _cookieService = cookieService;
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
            var data = new EmployerAccountData
            {
                CompanyNumber = model.CompanyNumber,
                CompanyName = model.CompanyName,
                DateOfIncorporation = model.DateOfIncorporation,
                RegisteredAddress = model.RegisteredAddress
            };

            var json = JsonConvert.SerializeObject(data);

            _cookieService.Create(HttpContext, CookieName, json, 365);

            return View(model);
        }

        public ActionResult Gateway()
        {
            return View();
        }
    }
}