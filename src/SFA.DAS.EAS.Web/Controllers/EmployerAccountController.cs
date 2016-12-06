﻿using System;
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

            var cookieData = _employerAccountOrchestrator.GetCookieData(HttpContext);
            var hideBreadcrumb = false;
            if (cookieData != null)
            {
                hideBreadcrumb = cookieData.HideBreadcrumb;
            }
            if (hideBreadcrumb == false)
            {
                hideBreadcrumb = TempData.ContainsKey("HideBreadcrumb") && (bool) TempData["HideBreadcrumb"];

                if (hideBreadcrumb)
                {
                    TempData["HideBreadcrumb"] = true;
                }
            }


            _employerAccountOrchestrator.DeleteCookieData(HttpContext);

            var model = new OrchestratorResponse<SelectEmployerViewModel>
            {
                Data = new SelectEmployerViewModel
                {
                    HideBreadcrumb = hideBreadcrumb
                }
            };

            return View(model);
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
                    RegisteredAddress = model.RegisteredAddress,
                    HideBreadcrumb = model.HideBreadcrumb
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
                    RegisteredAddress = existingData.RegisteredAddress,
                    HideBreadcrumb = existingData.HideBreadcrumb
                };
            }
            
            _employerAccountOrchestrator.CreateCookieData(HttpContext, data);
            var flashMessageViewModel = new FlashMessageViewModel();
            if (!string.IsNullOrEmpty(TempData["FlashMessage"]?.ToString()))
            {
                flashMessageViewModel = JsonConvert.DeserializeObject<FlashMessageViewModel>(TempData["FlashMessage"].ToString());
            }

            var gatewayInformViewModel = new OrchestratorResponse<GatewayInformViewModel>
            {
                Data = new GatewayInformViewModel
                {
                    BreadcrumbDescription = "Back to Your User Profile",
                    BreadcrumbUrl = Url.Action("Index","Home"),
                    ConfirmUrl = Url.Action("Gateway","EmployerAccount"),
                    HideBreadcrumb = data.HideBreadcrumb
                },
                FlashMessage = flashMessageViewModel
            };

            return View(gatewayInformViewModel);
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

                return RedirectToAction("GatewayInform");
            }

            var email = OwinWrapper.GetClaimValue("email");

            var empref = await _employerAccountOrchestrator.GetHmrcEmployerInformation(response.Data.AccessToken, email);
            
            var enteredData = _employerAccountOrchestrator.GetCookieData(HttpContext);

            enteredData.EmployerRef = empref.Empref;
            enteredData.AccessToken = response.Data.AccessToken;
            enteredData.RefreshToken = response.Data.RefreshToken;
            enteredData.EmpRefNotFound = empref.EmprefNotFound;
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
                EmpRefNotFound = enteredData.EmpRefNotFound,
                HideBreadcrumb = enteredData.HideBreadcrumb
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

            if (TempData.ContainsKey("HideBreadcrumb"))
            {
                TempData.Remove("HideBreadcrumb");
            }

            TempData["employerAccountCreated"] = "true";
            TempData["successHeader"] = "Levy account created";
            TempData["successMessage"] = $"You need to <a href=\"{ Url.Action("Index", "EmployerAgreement", new { response.Data.EmployerAgreement.HashedAccountId }) }\">sign the SFA agreement</a> before you can start spending your levy";

            return RedirectToAction("Index", "EmployerTeam", new { response.Data.EmployerAgreement.HashedAccountId });
        }

        private string GetUserId()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }
    }
}