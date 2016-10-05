using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerAccountController : BaseController
    {
        private readonly EmployerAccountOrchestrator _employerAccountOrchestrator;
        
        public EmployerAccountController(IOwinWrapper owinWrapper, EmployerAccountOrchestrator employerAccountOrchestrator, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
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
        [ValidateAntiForgeryToken]
        public ActionResult Index(string understood)
        {
            if (!string.IsNullOrEmpty(understood))
                return RedirectToAction("SelectEmployer");

            TempData["notunderstood"] = true;
            return View();
        }

        [HttpGet]
        public ActionResult SelectEmployer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectEmployer(SelectEmployerModel model)
        {
            var response = await _employerAccountOrchestrator.GetCompanyDetails(model);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;
                TempData["companyNumberError"] = true;
                return View(response);
            }

            return RedirectToAction("Gateway", response.Data);
        }

        [HttpGet]
        public async Task<ActionResult> Gateway(SelectEmployerViewModel model)
        {
            var data = new EmployerAccountData
            {
                CompanyNumber = model.CompanyNumber,
                CompanyName = model.CompanyName,
                DateOfIncorporation = model.DateOfIncorporation,
                RegisteredAddress = model.RegisteredAddress
            };

            _employerAccountOrchestrator.CreateCookieData(HttpContext, data);

            return Redirect(await _employerAccountOrchestrator.GetGatewayUrl(Url.Action("GateWayResponse","EmployerAccount",null,Request.Url.Scheme)));
        }

        public async Task<ActionResult> GateWayResponse()
        {
            var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
            if (response.Status != HttpStatusCode.OK)
            {
                response.Status = HttpStatusCode.OK;
                
                TempData["FlashMessage"] = JsonConvert.SerializeObject(response.FlashMessage);

                return RedirectToAction("Index","Home");
            }

            var email = OwinWrapper.GetClaimValue("email");

            var empref = await _employerAccountOrchestrator.GetHmrcEmployerInformation(response.Data.AccessToken, email);
            
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            enteredData.EmployerRef = empref.Empref;
            enteredData.AccessToken = response.Data.AccessToken;
            enteredData.RefreshToken = response.Data.RefreshToken;
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

        [AcceptVerbs(HttpVerbs.Get |HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ViewAccountAgreement()
        {
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            var model = new CreateAccountModel
            {
                CompanyNumber = enteredData.CompanyNumber,
                CompanyName = enteredData.CompanyName,
                CompanyRegisteredAddress = enteredData.RegisteredAddress,
                CompanyDateOfIncorporation = enteredData.DateOfIncorporation,
            };

            var response = await _employerAccountOrchestrator.GetAccountAgreementTemplate(model);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccount(bool? userIsAuthorisedToSign, string submit)
        {
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            var request = new CreateAccountModel
            {
                UserId = GetUserId(),
                CompanyNumber = enteredData.CompanyNumber,
                CompanyName = enteredData.CompanyName,
                CompanyRegisteredAddress = enteredData.RegisteredAddress,
                CompanyDateOfIncorporation = enteredData.DateOfIncorporation,
                EmployerRef = enteredData.EmployerRef,
                AccessToken = enteredData.AccessToken,
                RefreshToken = enteredData.RefreshToken,
                UserIsAuthorisedToSign = userIsAuthorisedToSign ?? false,
                SignedAgreement = submit.Equals("Sign", StringComparison.CurrentCultureIgnoreCase),
            };

            var response = await _employerAccountOrchestrator.CreateAccount(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;

                TempData["userNotAuthorised"] = "true";
                
                return RedirectToAction("ViewAccountAgreement");
            }

            if (request.UserIsAuthorisedToSign && request.SignedAgreement)
            {
                TempData["successHeader"] = $"Account created for { enteredData.CompanyName}";
                TempData["successMessage"] = "This account can now spend levy funds";
            }
            else
            {
                TempData["successHeader"] = $"Account created for { enteredData.CompanyName}";
                TempData["successMessage"] = "To spend the levy funds somebody needs to sign the agreement";
            }

            return RedirectToAction("Index", "EmployerTeam", new {accountId =  response.Data.EmployerAgreement.HashedId});
        }

        private string GetUserId()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }
    }
}