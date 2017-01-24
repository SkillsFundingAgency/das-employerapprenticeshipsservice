using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [AuthoriseActiveUser]
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

            return RedirectToAction("AddOrganisation", "EmployerAccountOrganisation");

        }

        [HttpGet]
        public ActionResult GatewayInform()
        {
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

            enteredData.EmployerRefName = empref.EmployerLevyInformation?.Employer?.Name?.EmprefAssociatedName ?? "";
            enteredData.PayeReference = empref.Empref;
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
                OrganisationType = enteredData.OrganisationType,
                OrganisationName = enteredData.OrganisationName,
                RegisteredAddress = enteredData.OrganisationRegisteredAddress,
                OrganisationReferenceNumber = enteredData.OrganisationReferenceNumber,
                OrganisationDateOfInception = enteredData.OrganisationDateOfInception,
                PayeReference = enteredData.PayeReference,
                EmployerRefName = enteredData.EmployerRefName,
                EmpRefNotFound = enteredData.EmpRefNotFound,
                OrganisationStatus = enteredData.OrganisationStatus,
                PublicSectorDataSource = enteredData.PublicSectorDataSource
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LegalAgreement()
        {
            return View();
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
                OrganisationType = enteredData.OrganisationType,
                OrganisationReferenceNumber = enteredData.OrganisationReferenceNumber,
                OrganisationName = enteredData.OrganisationName,
                OrganisationAddress = enteredData.OrganisationRegisteredAddress,
                OrganisationDateOfInception = enteredData.OrganisationDateOfInception,
                PayeReference = enteredData.PayeReference,
                AccessToken = enteredData.AccessToken,
                RefreshToken = enteredData.RefreshToken,
                OrganisationStatus = string.IsNullOrWhiteSpace(enteredData.OrganisationStatus) ? null : enteredData.OrganisationStatus,
                EmployerRefName = enteredData.EmployerRefName,
                PublicSectorDataSource = enteredData.PublicSectorDataSource
            };

            var response = await _employerAccountOrchestrator.CreateAccount(request, HttpContext);
            
            if (response.Status == HttpStatusCode.BadRequest)
            {
                response.Status = HttpStatusCode.OK;
                response.FlashMessage = new FlashMessageViewModel {Headline = "There was a problem creating your account"};
                return RedirectToAction("Summary");
            }
            
            TempData["employerAccountCreated"] = enteredData.OrganisationType.ToString();
            TempData["successHeader"] = "Account created";
            
            return RedirectToAction("Index", "EmployerTeam", new { response.Data.EmployerAgreement.HashedAccountId });
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(string hashedAccountId)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            var vm = await _employerAccountOrchestrator.GetRenameEmployerAccountViewModel(hashedAccountId, userIdClaim);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/rename")]
        public async Task<ActionResult> RenameAccount(RenameEmployerAccountViewModel vm)
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            var response = await _employerAccountOrchestrator.RenameEmployerAccount(vm, userIdClaim);

            if (response.Status == HttpStatusCode.OK)
            {
                var flashmessage = new FlashMessageViewModel
                {
                    Headline = "Account renamed",
                    Message = "You successfully updated the account name",
                    Severity = FlashMessageSeverityLevel.Success
                };

                TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

                return RedirectToAction("Index", "EmployerTeam");
            }

            var errorResponse = new OrchestratorResponse<RenameEmployerAccountViewModel>();

            if (response.Status == HttpStatusCode.BadRequest)
            {
                vm.ErrorDictionary = response.FlashMessage.ErrorMessages;
            }

            errorResponse.Data = vm;
            errorResponse.FlashMessage = response.FlashMessage;
            errorResponse.Status = response.Status;

            return View(errorResponse);
        }


        private string GetUserId()
        {
            var userIdClaim = OwinWrapper.GetClaimValue(@"sub");
            return userIdClaim ?? "";
        }
    }
}