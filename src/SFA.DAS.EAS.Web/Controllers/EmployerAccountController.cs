using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
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
        
        [HttpGet]
        public ActionResult SelectEmployer()
        {
            _employerAccountOrchestrator.DeleteCookieData(HttpContext);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectEmployer(SelectEmployerModel model)
        {
            var response = await _employerAccountOrchestrator.GetCompanyDetails(model);

            if (response.Status == HttpStatusCode.OK)
                return RedirectToAction("GatewayInform", response.Data);

            TempData["companyNumberError"] = "No company found. Please try again";
            response.Status = HttpStatusCode.OK;

            return View(response);
        }

        [HttpGet]
        public ActionResult GatewayInform(SelectEmployerViewModel model)
        {

            EmployerAccountData data;
            if (model?.CompanyName != null)
            {
                data = new EmployerAccountData
                {
                    CompanyNumber = model.CompanyNumber,
                    CompanyName = model.CompanyName,
                    DateOfIncorporation = model.DateOfIncorporation,
                    RegisteredAddress = model.RegisteredAddress
                };
            }
            else
            {
                var existingData = _employerAccountOrchestrator.GetCookieData(HttpContext);

                data = new EmployerAccountData
                {
                    CompanyNumber = existingData.CompanyNumber,
                    CompanyName = existingData.CompanyName,
                    DateOfIncorporation = existingData.DateOfIncorporation,
                    RegisteredAddress = existingData.RegisteredAddress
                };
            }
            
            _employerAccountOrchestrator.CreateCookieData(HttpContext, data);

            return View();
        }
        
        [HttpGet]
        public async Task<ActionResult> Gateway()
        {
            return Redirect(await _employerAccountOrchestrator.GetGatewayUrl(Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme)));
        }

        public async Task<ActionResult> GateWayResponse()
        {
            var response = await _employerAccountOrchestrator.GetGatewayTokenResponse(Request.Params["code"], Url.Action("GateWayResponse", "EmployerAccount", null, Request.Url.Scheme), System.Web.HttpContext.Current?.Request.QueryString);
            if (response.Status != HttpStatusCode.OK)
            {
                response.Status = HttpStatusCode.OK;

                TempData["FlashMessage"] = JsonConvert.SerializeObject(response.FlashMessage);

                return RedirectToAction("Index", "Home");
            }

            var email = OwinWrapper.GetClaimValue("email");

            var empref = await _employerAccountOrchestrator.GetHmrcEmployerInformation(response.Data.AccessToken, email);
            
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            enteredData.EmployerRef = empref.Empref;
            enteredData.AccessToken = response.Data.AccessToken;
            enteredData.RefreshToken = response.Data.RefreshToken;
            enteredData.EmprefNotFound = empref.EmprefNotFound;
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
                EmployerRef = enteredData.EmployerRef,
                EmprefNotFound = enteredData.EmprefNotFound
            };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccount()
        {
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            if (enteredData == null)
                return RedirectToAction("SelectEmployer", "EmployerAccount");

            var request = new CreateAccountModel
            {
                UserId = GetUserId(),
                CompanyNumber = enteredData.CompanyNumber,
                CompanyName = enteredData.CompanyName,
                CompanyRegisteredAddress = enteredData.RegisteredAddress,
                CompanyDateOfIncorporation = enteredData.DateOfIncorporation,
                EmployerRef = enteredData.EmployerRef,
                AccessToken = enteredData.AccessToken,
                RefreshToken = enteredData.RefreshToken
            };

            var response = await _employerAccountOrchestrator.CreateAccount(request, HttpContext);
            
            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;
                response.FlashMessage = new FlashMessageViewModel {Headline = "There was a problem creating your account"};
                return RedirectToAction("Summary");
            }

            TempData["employerAccountCreated"] = "true";
            TempData["successHeader"] = $"Account created for { enteredData.CompanyName}";
            TempData["successMessage"] = "You can now invite team members and spend your levy";

            return RedirectToAction("Index", "EmployerTeam", new { accountId = response.Data.EmployerAgreement.HashedId });
        }

        private string GetUserId()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }
    }
}