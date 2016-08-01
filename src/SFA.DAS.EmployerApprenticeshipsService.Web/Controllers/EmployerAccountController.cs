using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountController : Controller
    {
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        private readonly ICookieService _cookieService;

        public EmployerAccountController(IOwinWrapper owinWrapper, EmployerAccountOrchestrator employerAccountOrchestrator, ICookieService cookieService)
        {
            if (employerAccountOrchestrator == null)
                throw new ArgumentNullException(nameof(employerAccountOrchestrator));
            if (cookieService == null)
                throw new ArgumentNullException(nameof(cookieService));
            _owinWrapper = owinWrapper;
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

        [HttpGet]
        public async Task<ActionResult> Gateway()
        {
            return Redirect(await _employerAccountOrchestrator.GetGatewayUrl(Url.Action("GateWayResponse","EmployerAccount",null,Request.Url.Scheme)));
        }

        public async Task<ActionResult> GateWayResponse()
        {
            var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme));


            var data = GetEmployerAccountData();

            var email = _owinWrapper.GetClaimValue(ClaimTypes.Email);
            

            var selected = data.Data.FirstOrDefault(x => string.Equals(x.Email, email, StringComparison.CurrentCultureIgnoreCase));

            if (selected == null)
                return View("Gateway");

            var enteredData = GetCookieData();

            enteredData.EmployerRef = selected.EmpRef;
            enteredData.AccessToken = response.AccessToken;
            enteredData.RefreshToken = response.RefreshToken;

            _cookieService.Update(HttpContext, CookieName, JsonConvert.SerializeObject(enteredData));


            return RedirectToAction("Summary");
        }

        [HttpPost]
        public ActionResult Gateway(GatewayModel model)
        {
            var data = GetEmployerAccountData();

            var selected = data.Data.FirstOrDefault(x => x.Email == model.Email);

            if (selected == null)
                return View();

            var enteredData = GetCookieData();

            enteredData.EmployerRef = selected.EmpRef;

            _cookieService.Update(HttpContext, CookieName, JsonConvert.SerializeObject(enteredData));

            return RedirectToAction("Summary");
        }

        [HttpGet]
        public ActionResult Summary()
        {
            var enteredData = GetCookieData();

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
            var enteredData = GetCookieData();

            await _employerAccountOrchestrator.CreateAccount(new CreateAccountModel
            {
                UserId = GetUserId(),
                CompanyNumber = enteredData.CompanyNumber,
                CompanyName = enteredData.CompanyName,
                EmployerRef = enteredData.EmployerRef
            });

            return RedirectToAction("Index", "Home");
        }

        private string GetUserId()
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }

        private EmployerAccountData GetCookieData()
        {
            var cookie = (string)_cookieService.Get(HttpContext, CookieName);

            return JsonConvert.DeserializeObject<EmployerAccountData>(cookie);
        }

        private GatewayEmployers GetEmployerAccountData()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/EmpRefs/empref_data.json");

            var json = System.IO.File.ReadAllText(path);

            return JsonConvert.DeserializeObject<GatewayEmployers>(json);
        }

        public class GatewayEmployers
        {
            public List<GatewayEmployer> Data { get; set; }
        }

        public class GatewayEmployer
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string EmpRef { get; set; }
        }
    }
}