using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountController : Controller
    {
        
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        
        public EmployerAccountController(IOwinWrapper owinWrapper, EmployerAccountOrchestrator employerAccountOrchestrator)
        {
            if (employerAccountOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountOrchestrator));
            
            _owinWrapper = owinWrapper;
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
            var data = new EmployerAccountData
            {
                CompanyNumber = model.CompanyNumber,
                CompanyName = model.CompanyName,
                DateOfIncorporation = model.DateOfIncorporation,
                RegisteredAddress = model.RegisteredAddress
            };

            _employerAccountOrchestrator.CreateCookieData(HttpContext,data);
            
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Gateway()
        {
            return Redirect(await _employerAccountOrchestrator.GetGatewayUrl(Url.Action("GateWayResponse","EmployerAccount",null,Request.Url.Scheme)));
        }

        public async Task<ActionResult> GateWayResponse()
        {
            var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme));

            var empref = await _employerAccountOrchestrator.GetHmrcEmployerInformation(response.AccessToken);
            
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            enteredData.EmployerRef = empref != null ? empref.Empref : $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}";
            enteredData.AccessToken = response.AccessToken;
            enteredData.RefreshToken = response.RefreshToken;
            if (empref != null)
            {
                enteredData.CompanyName = $"{enteredData.CompanyName} - {empref.EmployerLevyInformation.Employer.Name.EmprefAssociatedName}";
            }

            _employerAccountOrchestrator.UpdateCookieData(HttpContext, enteredData);

            return RedirectToAction("Summary");
        }
        

        [HttpGet]
        public ActionResult Summary()
        {
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            var model = new SummaryViewModel
            {
                CompanyName = enteredData.CompanyName,
                CompanyNumber = enteredData.CompanyNumber,
                DateOfIncorporation = enteredData.DateOfIncorporation,
                EmployerRef = enteredData.EmployerRef
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount()
        {

            if (Request.Params[@"confirm_create"] != @"1")
            {
                TempData["ErrorMessage"] = "You can start the create account proccess again";
                return RedirectToAction("Index");
            }
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            await _employerAccountOrchestrator.CreateAccount(new CreateAccountModel
            {
                UserId = GetUserId(),
                CompanyNumber = enteredData.CompanyNumber,
                CompanyName = enteredData.CompanyName,
                EmployerRef = enteredData.EmployerRef
            });

            TempData["successHeader"] = "Account Created";
            TempData["successCompany"] = enteredData.CompanyName;

            return RedirectToAction("Index", "Home");
        }

        private string GetUserId()
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }
        
        
    }
}